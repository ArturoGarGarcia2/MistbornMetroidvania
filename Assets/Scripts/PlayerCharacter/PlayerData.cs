using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerData{
    int fileId;
    int maxHealth;
    int health;
    int checkpoint;
    int damage;
    int vials;
    int usedVials;
    int vialPower;
    string phase;
    int phaseTime;

    int[] unlockedAloMetals = new int[17];
    int[] unlockedFeruMetals = new int[17];
    int[] unlockedHemaMetals = new int[17];

    AloMetal[] aloMetals = new AloMetal[17];
    FeruMetal[] feruMetals = new FeruMetal[17];
    HemaMetal[] hemaMetals = new HemaMetal[17];

    int[] aloSlots = new int[8];
    int[] feruSlots = new int[4];
    int[] hemaSlots = new int[7];

    int eventNum;
    List<Event> achievedEvents = new List<Event>();

    public PlayerData(int fileId){
        this.fileId = fileId;
        Dictionary<string,object> fileData = DatabaseManager.Instance.GetSingleRowFromQuery(
            $"SELECT * FROM file WHERE id = {fileId};"
        );

        maxHealth = Convert.ToInt32(fileData["max_health"]);
        health = maxHealth;
        checkpoint = Convert.ToInt32(fileData["checkpoint_id"]);
        damage = Convert.ToInt32(fileData["damage"]);
        vials = Convert.ToInt32(fileData["vials"]);
        usedVials = 0;
        vialPower = Convert.ToInt32(fileData["vial_power"]);
        phase = fileData["phase"].ToString();
        phaseTime = Convert.ToInt32(fileData["phase_time"]);

        eventNum = DatabaseManager.Instance.GetInt("SELECT COUNT(*) FROM event");
        // for(int i = 1; i <= eventNum; i++){
        //     Event e = new Event(i);
        //     events.Add(e);
        //     Debug.Log($"event: {e}");
        // }
        int achievedEventNum = DatabaseManager.Instance.GetInt($"SELECT COUNT(*) FROM achieved_event WHERE file_id = {fileId}");
        for(int i = 1; i <= achievedEventNum; i++){
            Event e = new Event(i);
            achievedEvents.Add(e);
            Debug.Log($"event: {e}");
        }

        // Debug.Log($"VIALS: {vials}, USED VIALS: {usedVials}, REMAINING VIALS: {GetRemainingVials()}");

        SetMetals();

        // Debug.Log($"DATA: {this.ToString()}");
        // DebugMetals();
    }

    private void SetMetals(){
        for (int i = 1; i <= 17; i++) {
            Dictionary<string,object> row = DatabaseManager.Instance.GetSingleRowFromQuery(
                $"SELECT * FROM metal_file WHERE file_id = {fileId} AND metal_id = {i};"
            );

            if (row != null) {
                int metalId = Convert.ToInt32(row["metal_id"]);

                unlockedAloMetals[i-1] = Convert.ToInt32(row["unlocked_a"]);
                unlockedFeruMetals[i-1] = Convert.ToInt32(row["unlocked_f"]);
                unlockedHemaMetals[i-1] = Convert.ToInt32(row["unlocked_h"]);

                aloMetals[i-1] = new AloMetal(
                    (Metal)metalId,
                    Convert.ToInt32(row["amount_a"]),
                    Convert.ToInt32(row["capacity_a"]),
                    Convert.ToInt32(row["burning_rate"])
                );

                feruMetals[i-1] = new FeruMetal(
                    (Metal)metalId,
                    Convert.ToInt32(row["amount_f"]),
                    Convert.ToInt32(row["capacity_f"]),
                    Convert.ToInt32(row["storing_rate"]),
                    Convert.ToInt32(row["tapping_rate"])
                );

                hemaMetals[i-1] = new HemaMetal((Metal)metalId);

                // Leer los slots activos
                if (row.ContainsKey("slot_a") && row["slot_a"] != DBNull.Value) {
                    int slot = Convert.ToInt32(row["slot_a"]);
                    if (slot >= 1 && slot <= 8) aloSlots[slot - 1] = metalId;
                }

                if (row.ContainsKey("slot_f") && row["slot_f"] != DBNull.Value) {
                    int slot = Convert.ToInt32(row["slot_f"]);
                    if (slot >= 1 && slot <= 4) feruSlots[slot - 1] = metalId;
                }

                if (row.ContainsKey("slot_h") && row["slot_h"] != DBNull.Value) {
                    int slot = Convert.ToInt32(row["slot_h"]);
                    if (slot >= 1 && slot <= 7) hemaSlots[slot - 1] = metalId;
                }
            }
        }
    }

    public AloMetal GetMetalById(int idMetal){
        return aloMetals[idMetal-1];
    }

    public void EquipAloMetal(int metalId){
        if(!IsAloMetalUnlocked(metalId))return;
        int slotIndex = Array.IndexOf(aloSlots, metalId);
        if (slotIndex >= 0){
            aloSlots[slotIndex] = 0;
        }else{
            for (int i = 0; i < aloSlots.Length; i++){
                if(aloSlots[i]<1){
                    aloSlots[i] = metalId;
                    return;
                }
            }
        }
    }
    public void ChangeAloMetalBurning(int slot){
        AloMetal am = GetAloMetalInSlot(slot);
        if( am != null ){
            am.AlterBurning();
        }
    }

    public void EquipFeruMetal(int metalId){
        if(!IsFeruMetalUnlocked(metalId))return;
        int slotIndex = Array.IndexOf(feruSlots, metalId);
        if (slotIndex >= 0){
            feruSlots[slotIndex] = 0;
        }else{
            for (int i = 0; i < feruSlots.Length; i++){
                if(feruSlots[i]<1){
                    feruSlots[i] = metalId;
                    return;
                }
            }
        }
    }
    public void ChangeFeruMetalStatus(int slot){
        FeruMetal fm = GetFeruMetalInSlot(slot);
        if( fm != null ){
            fm.NextStatus();
        }
    }
    public int GetFeruStatusInSlot(int slot){
        FeruMetal fm = GetFeruMetalInSlot(slot);
        if(fm == null) return 0;
        return GetFeruMetalInSlot(slot).GetStatus();
    }

    public void EquipHemaMetal(int metalId){
        if(!IsHemaMetalUnlocked(metalId))return;
        int slotIndex = Array.IndexOf(hemaSlots, metalId);
        if (slotIndex >= 0){
            hemaSlots[slotIndex] = 0;
        }else{
            for (int i = 0; i < hemaSlots.Length; i++){
                if(hemaSlots[i]<1){
                    hemaSlots[i] = metalId;
                    return;
                }
            }
        }
    }

    public bool IsAloMetalEquipped(int metalId){
        return GetEquippedAloSlot(metalId) == -1 ? false : true;
    }
    public bool IsFeruMetalEquipped(int metalId){
        return GetEquippedFeruSlot(metalId) == -1 ? false : true;
    }
    public bool IsHemaMetalEquipped(int metalId){
        return GetEquippedHemaSlot(metalId) == -1 ? false : true;
    }

    public int GetEquippedAloSlot(int metalId){
        for(int slot = 0; slot < aloSlots.Length; slot++){
            if(aloSlots[slot] == metalId){
                return slot;
            }
        }
        return -1;
    }
    public int GetEquippedFeruSlot(int metalId){
        for(int slot = 0; slot < feruSlots.Length; slot++){
            if(feruSlots[slot] == metalId){
                return slot;
            }
        }
        return -1;
    }
    public int GetEquippedHemaSlot(int metalId){
        for(int slot = 0; slot < hemaSlots.Length; slot++){
            if(hemaSlots[slot] == metalId){
                return slot;
            }
        }
        return -1;
    }

    public int GetNailNum(){
        int acu = 0;
        for(int i = 1; i <= 17; i++){
            acu += IsHemaMetalEquipped(i) ? 1 : 0;
        }
        return acu;
    }

    public void EnterFuente(){
        health = maxHealth;
        usedVials = 0;
        foreach(FeruMetal feruMetal in feruMetals){
            feruMetal.SetStatus(0);
        }
        SaveToDatabase();
    }
    public void UseVial(){
        if (GetRemainingVials()>0){
            usedVials++;
            health+=vialPower;
            if(health>maxHealth){
                health=maxHealth;
            }
        }
    }

    void DebugMetals(){
        Debug.Log("=== ALOMETALS ===");
        for (int i = 0; i < aloMetals.Length; i++){
            var metal = aloMetals[i];
            if (metal != null){
                Debug.Log($"[{i}] {metal}");
                int slotIndex = Array.IndexOf(aloSlots, (int)metal.GetMetal());
                if (slotIndex != -1){
                    Debug.Log($"-> Está en el slot alomántico #{slotIndex + 1}");
                }
            }
        }

        Debug.Log("=== FERUMETALS ===");
        for (int i = 0; i < feruMetals.Length; i++){
            var metal = feruMetals[i];
            if (metal != null){
                Debug.Log($"[{i}] {metal}");
                int slotIndex = Array.IndexOf(feruSlots, (int)metal.GetMetal());
                if (slotIndex != -1){
                    Debug.Log($"-> Está en el slot feruquímico #{slotIndex + 1}");
                }
            }
        }

        Debug.Log("=== HEMAMETALS ===");
        for (int i = 0; i < hemaMetals.Length; i++){
            var metal = hemaMetals[i];
            if (metal != null){
                Debug.Log($"[{i}] {metal}");
                int slotIndex = Array.IndexOf(hemaSlots, (int)metal.GetMetal());
                if (slotIndex != -1)
                {
                    Debug.Log($"-> Está en el slot hemalúrgico #{slotIndex + 1}");
                }
            }
        }
    }

    public AloMetal GetAloMetalInSlot(int slot) {
        int metalId = aloSlots[slot - 1];
        return metalId > 0 ? aloMetals[metalId - 1] : null;
    }
    public FeruMetal GetFeruMetalInSlot(int slot) {
        int metalId = feruSlots[slot - 1];
        return metalId > 0 ? feruMetals[metalId - 1] : null;
    }
    public HemaMetal GetHemaMetalInSlot(int slot) {
        int metalId = hemaSlots[slot - 1];
        return metalId > 0 ? hemaMetals[metalId - 1] : null;
    }

    public void AddEvent(string eventName){
        Event newEvent = new Event(eventName);
        if(!IsEventAchievedByEvent(newEvent)){
            achievedEvents.Add(newEvent);
        }
    }

    public void SaveToDatabase(){
        SaveAloMetals();
        SaveFeruMetals();
        SaveHemaMetals();
        SavePlayerGeneralData();
        SaveEventData();
    }

    public void SaveAloMetals(){
        foreach(AloMetal am in aloMetals){
            DatabaseManager.Instance.ExecuteNonQuery(
                $"UPDATE metal_file "+
                $"SET amount_a = {am.GetAmount()}, capacity_a = {am.GetCapacity()}, unlocked_a = {unlockedAloMetals[(int)am.GetMetal()-1]} "+
                $"WHERE file_id = {fileId} AND metal_id = {(int)am.GetMetal()};"
            );
        }
        for(int i = 1; i <= aloSlots.Length; i++){
            DatabaseManager.Instance.ExecuteNonQuery(
                $"UPDATE metal_file "+
                $"SET slot_a = {i} "+
                $"WHERE file_id = {fileId} AND metal_id = {(int)GetAloMetalInSlot(i).GetMetal()};"
            );
        }
    }
    public void SaveFeruMetals(){
        foreach(FeruMetal fm in feruMetals){
            DatabaseManager.Instance.ExecuteNonQuery(
                $"UPDATE metal_file "+
                $"SET amount_f = {fm.GetAmount()}, capacity_f = {fm.GetCapacity()}, unlocked_f = {unlockedFeruMetals[(int)fm.GetMetal()-1]} "+
                $"WHERE file_id = {fileId} AND metal_id = {(int)fm.GetMetal()};"
            );
        }
        for(int i = 1; i <= feruSlots.Length; i++){
            DatabaseManager.Instance.ExecuteNonQuery(
                $"UPDATE metal_file "+
                $"SET slot_f = {i} "+
                $"WHERE file_id = {fileId} AND metal_id = {(int)GetFeruMetalInSlot(i).GetMetal()};"
            );
        }
    }
    public void SaveHemaMetals(){
        foreach(HemaMetal hm in hemaMetals){
            DatabaseManager.Instance.ExecuteNonQuery(
                $"UPDATE metal_file "+
                $"SET unlocked_h = {unlockedFeruMetals[(int)hm.GetMetal()-1]} "+
                $"WHERE file_id = {fileId} AND metal_id = {(int)hm.GetMetal()};"
            );
        }
        for(int i = 1; i <= hemaSlots.Length; i++){
            DatabaseManager.Instance.ExecuteNonQuery(
                $"UPDATE metal_file "+
                $"SET slot_h = {i} "+
                $"WHERE file_id = {fileId} AND metal_id = {(int)GetAloMetalInSlot(i).GetMetal()};"
            );
        }
    }
    public void SavePlayerGeneralData(){
        DatabaseManager.Instance.ExecuteNonQuery(
            $"UPDATE file "+
            $"SET max_health = {maxHealth}, "+
            $"checkpoint_id = {checkpoint}, "+
            $"damage = {damage}, "+
            $"vials = {vials}, "+
            $"vial_power = {vialPower}, "+
            $"phase = '{phase}', "+
            $"phase_time = {phaseTime} "+
            $"WHERE id = {fileId};"
        );
    }
    public void SaveEventData(){
        for(int i = 1; i <= eventNum; i++){
            if(!IsEventAchievedByID(i)){
                bool notAlreadyAchieved = DatabaseManager.Instance.GetInt($"SELECT COUNT(*) FROM achieved_event WHERE file_id={fileId} AND event_id={i}") == 0;
                if(notAlreadyAchieved){
                    DatabaseManager.Instance.ExecuteNonQuery(
                        $"INSERT INTO achieved_event (file_id,event_id) VALUES ({fileId},{i})"
                    );
                }
            }
        }
    }

    public int GetFileId() => fileId;
    public int GetMaxHealth() => maxHealth;
    public int GetHealth() => health;
    public int GetCheckPoint() => checkpoint;
    public int GetDamage() => damage;
    public int GetVials() => vials;
    public int GetUsedVials() => usedVials;
    public int GetRemainingVials() => vials-usedVials;
    public int GetVialPower() => vialPower;
    public string GetPhase() => phase;
    public int GetPhaseTime() => phaseTime;

    public bool IsAloMetalUnlocked(int idMetal) => unlockedAloMetals[idMetal-1] == 1;
    public bool IsFeruMetalUnlocked(int idMetal) => unlockedFeruMetals[idMetal-1] == 1;
    public bool IsHemaMetalUnlocked(int idMetal) => unlockedHemaMetals[idMetal-1] == 1;

    public AloMetal[] GetAloMetals() => aloMetals;
    public FeruMetal[] GetFeruMetals() => feruMetals;
    public HemaMetal[] GetHemaMetals() => hemaMetals;
    
    public int[] GetAloSlots() => aloSlots;
    public int[] GetFeruSlots() => feruSlots;
    public int[] GetHemaSlots() => hemaSlots;

    public bool IsEventAchievedByID(int eventId) => achievedEvents.IndexOf(new Event(eventId)) != -1;
    public bool IsEventAchievedByName(string eventName) => achievedEvents.IndexOf(new Event(eventName)) != -1;
    public bool IsEventAchievedByEvent(Event eventInstance) => achievedEvents.IndexOf(eventInstance) != -1;


    public void SetMaxHealth(int mh) {this.maxHealth=mh;}
    public void SetHealth(int h) {this.health=h;}
    public void SetCheckPoint(int cp) {this.checkpoint=cp;}
    public void SetDamage(int d) {this.damage=d;}
    public void SetVials(int v) {this.vials=v;}
    public void SetVialPower(int vp) {this.vialPower=vp;}
    public void SetPhase(string p) {this.phase=p;}
    public void SetPhaseTime(int pt) {this.phaseTime=pt;}

    public override string ToString(){
        return $"PlayerData (File ID: {fileId}, Health: {health}/{maxHealth}, Damage: {damage}, Vials: {vials}, Vial Power: {vialPower}, Phase: {phase}, Phase Time: {phaseTime})";
    }
}
