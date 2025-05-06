using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerData{
    int fileId;
    int baseMaxHealth;
    int maxHealth;
    int health;
    int checkpoint;
    int baseDamage;
    int damage;
    int vials;
    int usedVials;
    int vialPower;
    string phase;
    int phaseTime;

    float baseWeight;
    float weight;

    float baseSpeed;
    float speed;

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

    bool alive = true;
    float baseITime = 1f;
    float ITime = 1f;
    bool invulnerable = false;

    public bool canGoldHeal = true;

    public PlayerData(int fileId){
        this.fileId = fileId;
        Dictionary<string,object> fileData = DatabaseManager.Instance.GetSingleRowFromQuery(
            $"SELECT * FROM file WHERE id = {fileId};"
        );

        baseMaxHealth = Convert.ToInt32(fileData["max_health"]);
        maxHealth = baseMaxHealth;
        health = baseMaxHealth;
        
        baseDamage = Convert.ToInt32(fileData["damage"]);
        damage = baseDamage;

        baseWeight = 70f;
        weight = baseWeight;

        baseSpeed = 5f;
        speed = baseSpeed;

        checkpoint = Convert.ToInt32(fileData["checkpoint_id"]);
        vials = Convert.ToInt32(fileData["vials"]);
        usedVials = 0;
        vialPower = Convert.ToInt32(fileData["vial_power"]);
        phase = fileData["phase"].ToString();
        phaseTime = Convert.ToInt32(fileData["phase_time"]);

        eventNum = DatabaseManager.Instance.GetInt("SELECT COUNT(*) FROM event");
        List<int> retrievedAchievedEvent = DatabaseManager.Instance.GetIntListFromQuery($"SELECT event_id FROM achieved_event WHERE file_id = {fileId}");
        foreach(int i in retrievedAchievedEvent){
            Event e = new Event(i);
            achievedEvents.Add(e);
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
        if(fm.GetMetal() == Metal.COPPER) return;
        Debug.Log($"CAMBIANDO EL ESTADO DE {fm.GetMetal()}");
        if( fm != null){
            fm.NextStatus();
        }
    }
    public int GetFeruStatusInSlot(int slot){
        FeruMetal fm = GetFeruMetalInSlot(slot);
        if(fm == null) return 0;
        return GetFeruMetalInSlot(slot).GetStatus();
    }
    
    public void ExecuteFeruAndHemaUpdates(){
        damage = baseDamage;
        // weight = baseWeight;
        speed = baseSpeed;
        ITime = baseITime;
        UpdateStatsByFeru();
        UpdateStatsByHema();
    }
    
    public void UpdateStatsByFeru(){
        for(int i = 1; i <= feruSlots.Length; i++){
            ExecuteFeruUpdate(GetFeruMetalInSlot(i));
        }
    }
    private void ExecuteFeruUpdate(FeruMetal fm){
        if(fm == null) return;

        switch(fm.GetMetal()){
            case Metal.IRON:
                weight = (GetFeruMetalIfEquipped((int)Metal.IRON).IsStoring() ? baseWeight*2f : GetFeruMetalIfEquipped((int)Metal.IRON).IsTapping() ? baseWeight*.5f : baseWeight*1f);
                break;

            case Metal.STEEL:
                speed *= (GetFeruMetalIfEquipped((int)Metal.STEEL).IsStoring() ? .5f : GetFeruMetalIfEquipped((int)Metal.STEEL).IsTapping() ? 1.5f : 1f);
                break;

            case Metal.TIN:
                Debug.Log("FERUCHEMIC TIN WIP");
                break;

            case Metal.PEWTER:
                damage += (GetFeruMetalIfEquipped((int)Metal.PEWTER).IsStoring() ? baseDamage/4 : GetFeruMetalIfEquipped((int)Metal.PEWTER).IsTapping() ? baseDamage/2 : 0);
                break;

            case Metal.ZINC:
                ITime *= (GetFeruMetalIfEquipped((int)Metal.ZINC).IsStoring() ? .5f : GetFeruMetalIfEquipped((int)Metal.ZINC).IsTapping() ? 1.5f : 1f);
                break;

            case Metal.BRASS:
                Debug.Log("FERUCHEMIC BRASS WIP");
                break;

            case Metal.COPPER:
                Debug.Log("FERUCHEMIC COPPER WIP");
                break;

            case Metal.BRONZE:
                Debug.Log("FERUCHEMIC BRONZE WIP");
                break;

            case Metal.CADMIUM:
                Debug.Log("FERUCHEMIC CADMIUM WIP");
                break;

            case Metal.BENDALLOY:
                Debug.Log("FERUCHEMIC BENDALLOY WIP");
                break;

            case Metal.GOLD:
                Debug.Log("FERUCHEMIC GOLD WIP");
                break;

            case Metal.ELECTRUM:
                Debug.Log("FERUCHEMIC ELECTRUM WIP");
                break;

            case Metal.CHROMIUM:
                Debug.Log("FERUCHEMIC CHROMIUM WIP");
                break;

            case Metal.NICROSIL:
                Debug.Log("FERUCHEMIC NICROSIL WIP");
                break;

            case Metal.ALUMINIUM:
                Debug.Log("FERUCHEMIC ALUMINIUM WIP");
                break;

            case Metal.DURALUMIN:
                Debug.Log("FERUCHEMIC DURALUMIN WIP");
                break;

            case Metal.ATIUM:
                Debug.Log("FERUCHEMIC ATIUM WIP");
                break;
        }
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
    public void UpdateStatsByHema(){
        for(int i = 1; i <= hemaSlots.Length; i++){
            ExecuteHemaUpdate(GetHemaMetalInSlot(i));
        }
    }
    private void ExecuteHemaUpdate(HemaMetal hm){
        if(hm == null) return;

        switch(hm.GetMetal()){
            // METALES QUE SE ACTIVAN CADA X TIEMPO
            case Metal.IRON:
                Debug.Log("HEMALURGIC IRON WIP");
                break;

            case Metal.STEEL:
                speed *= (IsHemaMetalEquipped((int)Metal.STEEL) ? IsHemaMetalEquipped((int)Metal.ELECTRUM) ? 2f : 1.5f : 1f);
                break;

            case Metal.TIN:
                Debug.Log("HEMALURGIC TIN WIP");
                break;

            case Metal.PEWTER:
                damage += (IsHemaMetalEquipped((int)Metal.STEEL) ? IsHemaMetalEquipped((int)Metal.ELECTRUM) ? baseDamage/2 : baseDamage/4 : 0);
                break;

            case Metal.ZINC:
                ITime *= (IsHemaMetalEquipped((int)Metal.ZINC) ? IsHemaMetalEquipped((int)Metal.ELECTRUM) ? 2f : 1.5f : 1f);
                break;

            case Metal.BRASS:
                Debug.Log("HEMALURGIC BRASS WIP");
                break;

            case Metal.COPPER:
                Debug.Log("HEMALURGIC COPPER WIP");
                break;

            case Metal.BRONZE:
                Debug.Log("HEMALURGIC BRONZE WIP");
                break;

            case Metal.CADMIUM:
                Debug.Log("HEMALURGIC CADMIUM WIP");
                break;

            case Metal.BENDALLOY:
                Debug.Log("HEMALURGIC BENDALLOY WIP");
                break;

            case Metal.GOLD:
                HemaGoldHeal(hm);
                break;

            case Metal.CHROMIUM:
                Debug.Log("HEMALURGIC CHROMIUM WIP");
                break;

            case Metal.NICROSIL:
                Debug.Log("HEMALURGIC NICROSIL WIP");
                break;

            case Metal.ALUMINIUM:
                Debug.Log("HEMALURGIC ALUMINIUM WIP");
                break;

            case Metal.DURALUMIN:
                Debug.Log("HEMALURGIC DURALUMIN WIP");
                break;

            case Metal.ATIUM:
                // Debug.Log("HEMALURGIC ATIUM WIP");
                break;
        }
    }

    public void HemaGoldHeal(HemaMetal hm){
        if(health<maxHealth && hm.IsActive()){
            health+=2;
            hm.Deactivate();
        }
        if(health>=maxHealth)
            health = maxHealth;
    }

    // private static readonly System.Random random = new System.Random();

    public void Hurt(int hurtDamage){
        if(IsHemaMetalEquipped((int)Metal.ATIUM)){
            if(UnityEngine.Random.Range(1,101) < (IsHemaMetalEquipped((int)Metal.ELECTRUM) ? 20 : 10)){
                Debug.Log("No vea qué suertudo");
                invulnerable = true;
                return;
            }
        }
        if(!invulnerable){
            health -= hurtDamage;
            if(health <= 0){
                alive = false;
            }
            invulnerable = true;
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

    public AloMetal GetAloMetalIfEquipped(int metalId){
        int slot = GetEquippedAloSlot(metalId);
        if(slot==-1) return null;
        AloMetal am = GetAloMetalInSlot(slot+1);
        if(am == null){
            return null;
        }
        return am;
    }
    public FeruMetal GetFeruMetalIfEquipped(int metalId){
        int slot = GetEquippedFeruSlot(metalId);
        if(slot==-1) return null;
        FeruMetal fm = GetFeruMetalInSlot(slot+1);
        if(fm == null){
            return null;
        }
        return fm;
    }
    public HemaMetal GetHemaMetalIfEquipped(int metalId){
        int slot = GetEquippedHemaSlot(metalId);
        if(slot==-1) return null;
        HemaMetal hm = GetHemaMetalInSlot(slot+1);
        if(hm == null){
            return null;
        }
        return hm;
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
        DatabaseManager.Instance.ExecuteNonQuery(
            $"UPDATE metal_file "+
            $"SET slot_a = 0 "+
            $"WHERE file_id = {fileId};"
        );
        for(int i = 1; i <= aloSlots.Length; i++){
            if(GetAloMetalInSlot(i) == null)continue;
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
        DatabaseManager.Instance.ExecuteNonQuery(
            $"UPDATE metal_file "+
            $"SET slot_f = 0 "+
            $"WHERE file_id = {fileId};"
        );
        for(int i = 1; i <= feruSlots.Length; i++){
            if(GetFeruMetalInSlot(i) == null)continue;
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
                $"SET unlocked_h = {unlockedHemaMetals[(int)hm.GetMetal()-1]} "+
                $"WHERE file_id = {fileId} AND metal_id = {(int)hm.GetMetal()};"
            );
        }
        DatabaseManager.Instance.ExecuteNonQuery(
            $"UPDATE metal_file "+
            $"SET slot_h = 0 "+
            $"WHERE file_id = {fileId};"
        );
        for(int i = 1; i <= hemaSlots.Length; i++){
            if(GetHemaMetalInSlot(i) == null)continue;
            DatabaseManager.Instance.ExecuteNonQuery(
                $"UPDATE metal_file "+
                $"SET slot_h = {i} "+
                $"WHERE file_id = {fileId} AND metal_id = {(int)GetHemaMetalInSlot(i).GetMetal()};"
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
        foreach(Event e in achievedEvents){
            bool notAlreadyAchieved = DatabaseManager.Instance.GetInt($"SELECT COUNT(*) FROM achieved_event WHERE file_id={fileId} AND event_id={e.GetId()}") < 1;
            if(notAlreadyAchieved){
                DatabaseManager.Instance.ExecuteNonQuery(
                    $"INSERT INTO achieved_event (file_id,event_id) VALUES ({fileId},{e.GetId()})"
                );
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
    public float GetWeight() => weight;
    public float GetSpeed() => speed;
    public float GetITime() => ITime;
    public bool IsInvulnerable() => invulnerable;

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


    public void SetMaxHealth(int mh) => this.maxHealth=mh;
    public void SetHealth(int h) => this.health=h;
    public void SetCheckPoint(int cp) => this.checkpoint=cp;
    public void SetDamage(int d) => this.damage=d;
    public void SetVials(int v) => this.vials=v;
    public void SetVialPower(int vp) => this.vialPower=vp;
    public void SetPhase(string p) => this.phase=p;
    public void SetPhaseTime(int pt) => this.phaseTime=pt;
    public void MakeInvulnerable() => invulnerable = true;
    public void MakeVulnerable() => invulnerable = false;

    public override string ToString(){
        return $"PlayerData (File ID: {fileId}, Health: {health}/{maxHealth}, Damage: {damage}, Vials: {vials}, Vial Power: {vialPower}, Phase: {phase}, Phase Time: {phaseTime})";
    }
}
