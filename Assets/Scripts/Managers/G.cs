using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class G : MonoBehaviour {
    [Header("Balancing")]
    public float attackDistance;
    public float collideDistance;
    public float commonDropChance;
    public float rareDropChance;
    public float leggyDropChance;

    [Header("State")]
    public List<Item> commonItems;
    public List<Item> rareItems;
    public List<Item> leggyItems;

    [Header("Prefabs")]
    public List<Item> items;
    public Sprite transparentSprite;
    
    [Header("Scene References")]
    public GameObject background;

    [Header("Colors")]
    public Color white = new Color(201, 204, 161);
    public Color yellow = new Color(202, 160, 90);
    public Color orange = new Color(174, 106, 71);
    public Color red = new Color(139, 64, 73);
    public Color black = new Color(84, 51, 68);
    public Color darkGrey = new Color(81, 82, 98);
    public Color grey = new Color(99, 120, 125);
    public Color lightGrey = new Color(142, 160, 145);
    
    public static G m;
    
    public enum SceneName { Battle, StartMenu }

    public void Start() {
        if (m == null) m = this;
        if (m != this) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);

        commonItems = items.Where(i => i.rarity == Item.Rarity.COMMON).ToList();
        rareItems = items.Where(i => i.rarity == Item.Rarity.RARE).ToList();
        leggyItems = items.Where(i => i.rarity == Item.Rarity.LEGGY).ToList();
    }

    public void StartGame() {
        background.SetActive(true);
        LoadScene(SceneName.Battle);
    }

    public void LoadScene(SceneName sceneName) {
        m.Wait(0.4f, () => SceneManager.LoadScene(sceneName.ToString()));
    }

    public bool itemsDepleted => commonItems.Count == 0 && rareItems.Count == 0 && leggyItems.Count == 0;

    public Item GetRandomItem() {
        Item.Rarity rarity = this.WeightedRandom(
            Item.Rarity.COMMON, (commonItems.Count == 0) ? 0 : commonDropChance, 
            Item.Rarity.RARE, (rareItems.Count == 0) ? 0 : rareDropChance,
            Item.Rarity.LEGGY, (leggyItems.Count == 0) ? 0 : leggyDropChance);

        Item pickedItem;
        if (rarity == Item.Rarity.COMMON) {
            pickedItem = commonItems.Random();
            commonItems.Remove(pickedItem);
        } else if (rarity == Item.Rarity.RARE) {
            pickedItem = rareItems.Random();
            rareItems.Remove(pickedItem);
        } else  {
            pickedItem = leggyItems.Random();
            leggyItems.Remove(pickedItem);
        }
        return pickedItem;
    }
}
