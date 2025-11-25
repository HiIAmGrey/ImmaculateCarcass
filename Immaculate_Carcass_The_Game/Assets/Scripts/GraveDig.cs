using UnityEngine;
using UnityEngine.SceneManagement;

public class GraveDig : MonoBehaviour
{
    public int graveID;
    public bool dug = false;

    void Start()
    {
        // If the player dug this grave earlier, keep it dug
        dug = GraveDigTracker.dugCount > graveID; 
        // OR store separate flags later
        if (dug)
        {
            SetToDugAppearance();
        }
    }

    void OnMouseDown()
    {
        // Can't dig if already dug
        if (dug) return;

        // Must have shovel
        if (!PlayerInventory.Instance.hasShovel)
        {
            Debug.Log("You need a shovel to dig this grave.");
            return;
        }

        dug = true;
        GraveDigTracker.dugCount++;

        SetToDugAppearance();

        // Load combat with correct encounter ID
        PersistentGameState.isOverworldEncounter = false;
        EnemyEncounterManager.SetEncounterID(GraveDigTracker.dugCount);

        SceneManager.LoadScene("CombatScene");
    }

    void SetToDugAppearance()
    {
        // DO: darken or change material
        GetComponent<MeshRenderer>().material.color = Color.gray;
    }
}
