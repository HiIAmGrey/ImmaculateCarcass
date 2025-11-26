using UnityEngine;

public class QuestNPC : MonoBehaviour
{
    public enum QuestState { NotStarted, InProgress, Completed, FinalBattle }
    public int gravesNeeded = 3;

    private QuestNPC_Manager qm;

    void Start()
    {
        qm = QuestNPC_Manager.Instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Use the persistent grave count 
        int dug = PersistentGameState.graveCount;

        QuestState state = (QuestState)qm.GetState();

        switch (state)
        {
            case QuestState.NotStarted:

                if (!PlayerInventory.Instance.hasShovel)
                {
                    DialogueManager.Instance.ShowDialogue(
                        "Ah... a living soul wandering this rotted parcel of land.\n" +
                        "Purpose eludes you... but tools do not.\n\n" +
                        "<i>You will need something to break the soil...</i>"
                    );
                    return;
                }

                DialogueManager.Instance.ShowDialogue(
                    "Ah… a living soul wandering this rotted parcel of land.\n" +
                    "If it is purpose you seek, then listen well.\n\n" +
                    "<color=#B089F0>Somewhere beneath these graves lies the Immaculate Carcass—\n" +
                    "a relic of purity untouched by the corruption.</color>\n\n" +
                    "Bring it to me, and I shall tell you why the dead whisper."
                );

                qm.SetState((int)QuestState.InProgress);
                break;

            case QuestState.InProgress:

                if (!PlayerInventory.Instance.hasShovel)
                {
                    DialogueManager.Instance.ShowDialogue(
                        "You'll need a tool… something to break the soil."
                    );
                    return;
                }

                if (dug == 0)
                {
                    DialogueManager.Instance.ShowDialogue(
                        "Do not idle.\n" +
                        "The carcass is buried in pieces throughout this plot.\n" +
                        "The soil remembers where it hides."
                    );
                }
                else if (dug == 1)
                {
                    DialogueManager.Instance.ShowDialogue(
                        "You’ve found the first remnant.\n" +
                        "Its aura resonates… keep digging.\n" +
                        "<i>The truth is near.</i>"
                    );
                }
                else if (dug == 2)
                {
                    DialogueManager.Instance.ShowDialogue(
                        "Another piece reclaimed.\n" +
                        "Your hands are becoming… familiar with the grave.\n" +
                        "One more remains."
                    );
                }
                else if (dug >= gravesNeeded)
                {
                    DialogueManager.Instance.ShowDialogue(
                        "Ah… you carry all three fragments.\n\n" +
                        "<color=#E47676>Bring them here, and let me gaze upon perfection.</color>"
                    );

                    qm.SetState((int)QuestState.Completed);
                }

                break;

            case QuestState.Completed:

                DialogueManager.Instance.ShowDialogue(
                    "Give them to me.\n" +
                    "The Immaculate Carcass belongs not to the dead… but to ME.\n\n" +
                    "*The NPC’s posture straightens. His tone curdles.*"
                );

                qm.SetState((int)QuestState.FinalBattle);
                break;

            case QuestState.FinalBattle:

                DialogueManager.Instance.ShowDialogue(
                    "<size=36><b>Fool.</b></size>\n\n" +
                    "Did you truly believe purity still existed in this place?\n" +
                    "You have unearthed nothing but power—MY power.\n\n" +
                    "<color=#FF6F6F>And now you will return it with your life.</color>"
                );

                FinalBossLoader.StartBossCombat();
                break;
        }
    }
}
