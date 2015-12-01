// Copyright (C) 2015 Jaroslav Stehlik - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#define IGNORE_EXCEPTIONS

#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SVGImporter 
{
    using Document;
    using Rendering;
    using Geometry;
    using Data;
    using Utils;

    public class SVGAssetImport
    {    
        public static List<SVGError> errors;	
        protected static bool _importingSVG = false;
        public static bool importingSVG
        {
            get {
                return _importingSVG;
            }
        }

        public static float vpm;
        private string _SVGFile;
        private Texture2D _texture = null;
        private SVGGraphics _graphics;
        private SVGDocument _svgDocument;
    
        public SVGAssetImport(string svgFile, float vertexPerMeter = 1000f)
        {
            vpm = vertexPerMeter;
            this._SVGFile = svgFile;
            _graphics = new SVGGraphics(vertexPerMeter);
        }

        private void CreateEmptySVGDocument()
        {
            _svgDocument = new SVGDocument(_SVGFile, this._graphics);
        }

        public void StartProcess(SVGAsset asset)
        {
            if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                return;

//            Debug.Log("Importing asset: "+UnityEditor.AssetDatabase.GetAssetPath(asset));

            if(errors == null)
            {
                errors = new List<SVGError>();
            } else {
                errors.Clear();
            }
            _importingSVG = true;

            UnityEditor.SerializedObject svgAsset = new UnityEditor.SerializedObject(asset);
//            UnityEditor.SerializedProperty gradientTexture = svgAsset.FindProperty("_gradientTexture");
            UnityEditor.SerializedProperty sharedMesh = svgAsset.FindProperty("_sharedMesh");            
            UnityEditor.SerializedProperty sharedShaders = svgAsset.FindProperty("_sharedShaders");

            // clean up
            SVGParser.Clear();
            SVGParser.Init();
            SVGGraphics.Clear();
            SVGGraphics.Init();

            SVGElement _rootSVGElement = null;

#if IGNORE_EXCEPTIONS
            try {
#endif
                // Create new Asset
                CreateEmptySVGDocument();
                _rootSVGElement = this._svgDocument.rootElement;
#if IGNORE_EXCEPTIONS
            } catch (System.Exception exception) {
                _rootSVGElement = null;
                errors.Add(SVGError.Syntax);
                Debug.LogError("SVG Document Exception: "+exception.Message);
            }
#endif

            if(_rootSVGElement == null)
            {
                Debug.LogError("SVG Document is corrupted! "+UnityEditor.AssetDatabase.GetAssetPath(asset));
                return;
            }

            SVGGraphics.depthTree = new SVGDepthTree(_rootSVGElement.paintable.viewport);

#if IGNORE_EXCEPTIONS
            try {
#endif
                _rootSVGElement.Render();

                SVGAtlas atlas = SVGAtlas.Instance;
                atlas.hideFlags = HideFlags.DontSave;
                if(SVGAtlas.Instance.gradients != null && atlas.gradients.Count > 1)
                {
                    int gradientCount = atlas.gradients.Count;
                    int gradientWidth = atlas.gradientWidth;
                    int gradientHeight = atlas.gradientHeight;
                    atlas.atlasTextureWidth = gradientWidth * 2;
                    atlas.atlasTextureHeight = Mathf.CeilToInt((gradientCount * gradientWidth) / atlas.atlasTextureWidth) * gradientHeight + gradientHeight;
                    atlas.RebuildAtlas();
                }

                Material[] outputMaterials;
                Mesh mesh = SVGMesh.CreateMesh(out outputMaterials);
                if(mesh == null)
                    return;

                Bounds bounds = mesh.bounds;
                Vector3[] vertices = mesh.vertices;
                Vector2 offset = new Vector2(bounds.min.x + bounds.size.x * asset.pivotPoint.x,
                                             bounds.min.y + bounds.size.y * asset.pivotPoint.y);

                // Apply pivot point and Flip Y Axis
				for(int i = 0; i < vertices.Length; i++)
                {
                    vertices[i].x = vertices[i].x - offset.x;
                    vertices[i].y = vertices[i].y - offset.y;
                    vertices[i].y *= -1f;
                }

                mesh.vertices = vertices;
                mesh.RecalculateBounds();
                sharedMesh.objectReferenceValue = AddObjectToAsset<Mesh>(mesh, asset, HideFlags.HideInHierarchy);

//                Material sharedMaterial;
                if(outputMaterials != null && outputMaterials.Length > 0)
                {
                    sharedShaders.arraySize = outputMaterials.Length;
                    for(int i = 0; i < outputMaterials.Length; i++)
                    {
                        sharedShaders.GetArrayElementAtIndex(i).stringValue = outputMaterials[i].shader.name;                                                
                    }
                }

                svgAsset.ApplyModifiedProperties();

                if(asset.generateCollider)
                {
                    // Create polygon contour
                    if(SVGGraphics.paths != null && SVGGraphics.paths.Count > 0)
                    {
                        List<List<Vector2>> polygons = new List<List<Vector2>>();
                        for(int i = 0; i < SVGGraphics.paths.Count; i++)
                        {
                            Vector2[] points = SVGGraphics.paths[i].points;
                            for(int j = 0; j < points.Length; j++)
                            {
                                points[j].x = points[j].x * SVGMesh.meshScale  - offset.x;
                                points[j].y = (points[j].y * SVGMesh.meshScale  - offset.y) * -1f;
                            }

                            polygons.Add(new List<Vector2>(points));
                        }
                        
                        polygons = SVGGeom.MergePolygon(polygons);
                        
                        SVGPath[] paths = new SVGPath[polygons.Count];
                        for(int i = 0; i < polygons.Count; i++)
                        {
                            paths[i] = new SVGPath(polygons[i].ToArray());
                        }
                        
                        MethodInfo _editor_SetColliderShape = typeof(SVGAsset).GetMethod("_editor_SetColliderShape", BindingFlags.NonPublic | BindingFlags.Instance);
                        if(paths != null && paths.Length > 0)
                        {
                            _editor_SetColliderShape.Invoke(asset, new object[]{paths});
                        } else {
                            _editor_SetColliderShape.Invoke(asset, new object[]{null});
                        }
                    }
                } else {
                    MethodInfo _editor_SetColliderShape = typeof(SVGAsset).GetMethod("_editor_SetColliderShape", BindingFlags.NonPublic | BindingFlags.Instance);
                    _editor_SetColliderShape.Invoke(asset, new object[]{null});
                }

                MethodInfo _editor_SetGradients = typeof(SVGAsset).GetMethod("_editor_SetGradients", BindingFlags.NonPublic | BindingFlags.Instance);
                if(atlas.gradients != null && atlas.gradients.Count > 0)
                {
                    _editor_SetGradients.Invoke(asset, new object[]{atlas.gradients.ToArray()});
                } else {
                    _editor_SetGradients.Invoke(asset, new object[]{null});
                }
#if IGNORE_EXCEPTIONS
            } catch(System.Exception exception) {
                Debug.LogWarning("Asset: "+UnityEditor.AssetDatabase.GetAssetPath(asset)+" Failed to import\n"+exception.Message);
                errors.Add(SVGError.CorruptedFile);
            }
#endif

            SVGAtlas.ClearAll();
            SVGGraphics.Clear();
            if(_svgDocument != null)
                _svgDocument.Clear();

            UnityEditor.EditorUtility.SetDirty(asset);
            _importingSVG = false;
        }

        protected T AddObjectToAsset<T>(T obj, SVGAsset asset, HideFlags hideFlags) where T : UnityEngine.Object
        {
            if(obj == null)
                return null;

            obj.hideFlags = hideFlags;
            UnityEditor.AssetDatabase.AddObjectToAsset(obj, asset);
            return obj;
        }

        public void NewSVGFile(string svgFile)
        {
            this._SVGFile = svgFile;
        }

        public Texture2D GetTexture()
        {
            if (this._texture == null)
            {
                return new Texture2D(0, 0, TextureFormat.ARGB32, false);
            }
            return this._texture;
        }

        public Texture2D CloneTexture(Texture2D texture)
        {
            if(texture == null)
                return null;

            Texture2D output = new Texture2D(texture.width, texture.height, texture.format, false);
            output.name = texture.name;
            output.SetPixels32(texture.GetPixels32());
            output.wrapMode = TextureWrapMode.Clamp;
            output.anisoLevel = 0;
            output.alphaIsTransparency = true;
            output.filterMode = FilterMode.Bilinear;
            output.Apply();
            return output;
        }    
    }
}
#endif