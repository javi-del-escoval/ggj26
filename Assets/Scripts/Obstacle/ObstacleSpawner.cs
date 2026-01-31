using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
	[Header("Prefabs")]
	public GameObject[] enemyVariants, wallVariants, FurnitureVariants;
	private List<GameObject> obstacles;
	[SerializeField] Transform[] lanes;
	public float cooldown { get; private set; }
	float timeElapsed;
	public static ObstacleSpawner Instance { get; private set; }
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			// If an instance already exists and it's not this one, destroy this duplicate
			Destroy(this.gameObject);
		}
		else
		{
			// Otherwise, set the instance to this object and ensure it persists across scenes
			Instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
	}
	void Start()
	{
		obstacles = new List<GameObject>();
		cooldown = 4/GameManager.Instance.difficulty;
	}

	void Update()
	{
		int laneIndex = Random.Range(0, lanes.Length);
		timeElapsed += Time.deltaTime;
		if(timeElapsed >= cooldown)
		{
			timeElapsed = 0;
			cooldown = 4/GameManager.Instance.difficulty;
			//spawn
			int typeIndex = Random.Range(0, 3);
			GameObject[] type;
			switch (typeIndex)
			{
				case 0:
					type = wallVariants;
					break;
				case 1:
					type = enemyVariants;
					break;
				case 2:
					type = FurnitureVariants;
					break;
				default:
					type = new GameObject[0];
					Debug.Log($"Failed to Spawn Obstacle, typeIndex: {typeIndex}");
					break;
			}
			int variantIndex = Random.Range(0, type.Length);
			Vector2 pos = (Vector2)lanes[laneIndex].position + new Vector2(12,0);
			obstacles.Add(Instantiate(type[variantIndex], pos, Quaternion.identity, transform));
		}
		foreach(GameObject obj in obstacles)
		{
			Vector2 target = new Vector2(-12, obj.transform.position.y);
			obj.transform.position = Vector2.MoveTowards(obj.transform.position, target, GameManager.Instance.speed*Time.deltaTime);
		}
	}

	public void RemoveObstacle(GameObject obj)
	{
		obstacles.Remove(obj);
		Destroy(obj);
	}
}
