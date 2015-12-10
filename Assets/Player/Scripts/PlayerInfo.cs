using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SVGImporter;

public class PlayerInfo : MonoBehaviour
{
	public static float maxTraveled;
	public GameObject Magnet_Active;
	public GameObject Bubble_Active;
	public GameObject JetPack_Active;


	private static List<GameObject> activeBonuses;
	private Dictionary<string, GameObject> bonuses;
	private Transform tr;

	public static bool IsInvulnerable { get; private set; }

	private bool faceLeft;
	private bool HaveLost;

	void Awake()
	{
//        enabled = false;
		//46859
		//паспорт
		//регистрация
		//паспорт т с
		//кредитный договор 1 и посл стр
		//копия птс
		//акт передачи птс в банк
		//скан осаго и каско
		//
		//
		//
		//фио, дата рожд, скан прав.
		//
		//до 19:00
		//
		tr = transform;
		maxTraveled = 0;
		activeBonuses = new List<GameObject>();
		bonuses = new Dictionary<string, GameObject>();
		bonuses[TagManager.JetPack] = GameObject.Find("JetPack_Active");//Game.MakePrefabInstance(JetPack_Active);
		bonuses[TagManager.Magnet] = GameObject.Find("Magnet_Active");//Game.MakePrefabInstance(Magnet_Active);
		bonuses[TagManager.Bubble] = GameObject.Find("Bubble_Active");//Game.MakePrefabInstance(Bubble_Active);
//        Game.RegisterPausableObject(this);
	}

	void Update()
	{

//        var currentY = transform.localPosition.y;
//        if (maxTraveled < currentY)
		maxTraveled = tr.localPosition.y;

		if (HaveLost)
			GameOver();
//
//        if (CheckGameOver())
//            GameOver();
	}

	void OnLevelWasLoaded(int level)
	{
		if (level != 1)
			enabled = false;
	}

	public void MoveTo(MoveDirection direction)
	{
		if (direction == MoveDirection.Left && !faceLeft || direction == MoveDirection.Right && faceLeft)
			Mirror();
	}


	void Mirror()
	{
		var scale = tr.localScale;
		tr.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
		faceLeft = !faceLeft;
	}


	void OnBecameInvisible()
	{
		HaveLost = true;
	}

	private bool CheckGameOver()
	{
		var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		if (!GeometryUtility.TestPlanesAABB(planes, GetComponent<Renderer>().bounds))
			return true; // Player not in camera      

		return false;
	}

	public void GameOver()
	{
		HaveLost = false;
		Game.GameOver();
	}


	public void AddBonus(GameObject bonus)
	{
		if (bonus.CompareTag(TagManager.JetPack))
			UseJetPack(bonus);
		else
		if (bonus.CompareTag(TagManager.Bubble))
			UseBubble(bonus);
		else
		if (bonus.CompareTag(TagManager.Magnet))
			UseMagnet(bonus);
	}


	public void MakeDamage()
	{
		if (HasBubble())
		{
			var bubbleBonus = activeBonuses.FirstOrDefault(b => b.CompareTag(TagManager.Bubble));
			var bubble = bonuses[TagManager.Bubble];
			StopBonusUsage(bubbleBonus, bubble);
			StartCoroutine(Invulnerable());
		}
		else
			GameOver();
	}


	public static bool HasJetPack()
	{
		return activeBonuses.Any(b => b.CompareTag(TagManager.JetPack));
	}


	public static bool HasBubble()
	{
		return activeBonuses.Any(b => b.CompareTag(TagManager.Bubble));
	}


	IEnumerator Invulnerable()
	{
		IsInvulnerable = true;
		bool isTransparent = false;
		for (int i = 0; i < 5 * 3; i++)
		{
			yield return new WaitForSeconds(0.2f * Time.timeScale);
			if (isTransparent)
				SetTransparency(1f);
			else
				SetTransparency(0.3f);

			isTransparent = !isTransparent;
		}
		SetTransparency(1f);
		IsInvulnerable = false;
	}

	void SetTransparency(float alpha)
	{
		var renderer = GetComponent<SVGRenderer>();
		var color = renderer.color;
//		var renderer = GetComponent<Renderer>();
//		var mat = renderer.material;
		renderer.color = new Color(color.r, color.g, color.b, alpha);
//		renderer.material.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
	}


	void UseJetPack(GameObject bonus)
	{
		var rigidBody2d = GetComponent<Rigidbody2D>();
		rigidBody2d.gravityScale = 0;
		rigidBody2d.velocity = new Vector2(rigidBody2d.velocity.x, 20);
		var jetPack = bonuses[TagManager.JetPack];//transform.Find("JetPackFlight").gameObject;
		StartBonusUsageCoroutine(bonus, jetPack, () => rigidBody2d.gravityScale = 1);
	}


	void UseBubble(GameObject bonus)
	{
		if (IsInvulnerable)
		{
			StopCoroutine("Invulnerable");
			IsInvulnerable = false;
		}
		var bubble = bonuses[TagManager.Bubble];//transform.Find("Bubble").gameObject;
		StartBonusUsageCoroutine(bonus, bubble);
	}

	void UseMagnet(GameObject bonus)
	{
		var magnet = bonuses[TagManager.Magnet];//transform.Find("MagnetField").gameObject;
		StartBonusUsageCoroutine(bonus, magnet);
	}


	void StartBonusUsageCoroutine(GameObject bonus, GameObject visual, System.Action onExpired = null)
	{        
		StartCoroutine(StartBonusUsage(bonus, visual, onExpired));
	}


	IEnumerator StartBonusUsage(GameObject bonus, GameObject visual, System.Action onExpired)
	{   
		EnableVisual(visual);
		PlatformBonusManager.BonusUsed(bonus);
		activeBonuses.Add(bonus);
		BonusPanel.AddBonus(bonus.tag);

		var duration = (float)PlayerPrefManager.GetDuration(bonus.tag) * Time.timeScale;
		var timeLeft = duration;
		var updateRate = 0.01f * Time.timeScale;
//        visual.SetActive(true);
		while (true)
		{
			if (!Game.IsPaused && !Game.IsGameOver)
			{
				if (timeLeft <= 0)
					break;
				else
				{
					timeLeft -= updateRate;
					BonusPanel.SetRemain(bonus.tag, timeLeft / duration);
				}
			}
			yield return new WaitForSeconds(updateRate);
//            yield return new WaitForSeconds(duration * Time.timeScale);
		}
		if (onExpired != null)
			onExpired();

		StopBonusUsage(bonus, visual);
	}

	void StopBonusUsage(GameObject bonus, GameObject visual)
	{
		BonusPanel.RemoveBonus(bonus.tag);
//        visual.SetActive(false);
		activeBonuses.Remove(bonus);
		PlatformBonusManager.BonusExpired(bonus);
		DisableVisual(visual);
	}

	void EnableVisual(GameObject visual)
	{
		visual.transform.localScale = new Vector3(1, 1, 1);
//        visual.transform.SetParent(this.tr);
//        visual.transform.localPosition = Vector3.zero;
		var boxCollider2d = visual.GetComponent<BoxCollider2D>();
		if (boxCollider2d != null)
			boxCollider2d.enabled = true;        
	}

	void DisableVisual(GameObject visual)
	{
		visual.transform.localScale = Vector3.zero;
		var boxCollider2d = visual.GetComponent<BoxCollider2D>();
		if (boxCollider2d != null)
			boxCollider2d.enabled = false;
//        visual.transform.SetParent(null);
	}
}