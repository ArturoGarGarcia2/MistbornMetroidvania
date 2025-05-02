using System;

public static class GameEvents {
    public static event Action<string> OnNPCSpokenTo;

    public static void NPCSpoken(string npcName) {
        OnNPCSpokenTo?.Invoke(npcName);
    }
}
