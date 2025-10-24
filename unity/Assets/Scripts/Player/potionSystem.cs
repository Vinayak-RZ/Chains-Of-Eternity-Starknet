using TMPro;
using UnityEngine;

public class potionSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject slot1;
    [SerializeField] private GameObject slot2;
    [SerializeField] private GameObject slot3;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private TextMeshProUGUI potioncount;
    private int potionCountValue = 0; // Initialize potion count value
    public void UsePotion(int slotNumber)
    {
        switch (slotNumber)
        {
            case 1:
                // Logic for using potion in slot 1
                Debug.Log("Using potion from slot 1");
                updatePotionText();
                break;
            case 2:
                // Logic for using potion in slot 2
                Debug.Log("Using potion from slot 2");
                updatePotionText();
                break;
            case 3:
                // Logic for using potion in slot 3
                Debug.Log("Using potion from slot 3");
                updatePotionText();
                break;
            default:
                Debug.LogWarning("Invalid slot number: " + slotNumber);
                break;
        }
    }

    private void updatePotionText()
    {
        potioncount = slot1.GetComponentInChildren<TextMeshProUGUI>();
        potionCountValue = int.Parse(potioncount.text); // Assuming the text is a valid integer
        potionCountValue--; // Decrease potion count
        potioncount.text = potionCountValue.ToString(); // Update the text to reflect the new count
    }
}
