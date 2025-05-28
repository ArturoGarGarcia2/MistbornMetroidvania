using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class PlayerData{
    int fileId;
    int baseMaxHealth;
    int maxHealth;
    int health;
    int checkpoint;
    Vector3 checkpointPosition;
    int baseDamage;
    int damage;
    int vials;
    int usedVials;
    int baseVialPower;
    int vialPower;
    string phase;
    int phaseTime;
    int coins;
    int metalVialNum;

    float baseWeight;
    float weight;

    float baseSpeed;
    float speed;

    float baseBreathCapacity;
    float breathCapacity;

    //El tiempo en empezar a quemarse
    float baseFireEntryTime;
    float fireEntryTime;
    //El tiempo en empezar a quemarse
    float baseFireExitTime;
    float fireExitTime;

    //El tiempo entre golpe y golpe de fuego
    float baseFireHitTime;
    float fireHitTime;

    float baseParryWindow;
    float parryWindow;

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

    bool canGoldHeal = true;

    MetalVial[] metalVials = new MetalVial[8];
    RawMetal[] rawMetals = new RawMetal[17];

    Laudano laudano;

    Projectile[] projectiles = new Projectile[3];

    Consumible[] inventorySlots = new Consumible[8];

    public Sprite LoadSubSprite(string sheetPath, string subSpriteName) {
        Sprite[] sprites = Resources.LoadAll<Sprite>(sheetPath);
        return sprites.FirstOrDefault(s => s.name == subSpriteName);
    }

    void RetrieveInventory(){
        for(int i = 1; i <= metalVials.Length; i++){
            List<MetalVialContent> contents = new List<MetalVialContent>();
            List<Dictionary<string, object>> metalVialContents = DatabaseManager.Instance.GetRowsFromQuery(
                $"SELECT * FROM metal_vial_content WHERE vial_slot = {i} AND file_id = {fileId}"
            );

            foreach(Dictionary<string, object> contentData in metalVialContents){
                int amount = Convert.ToInt32(contentData["amount"]);
                Metal metal = (Metal)(Convert.ToInt32(contentData["metal_id"]));
                contents.Add(new MetalVialContent(metal,amount));
            }
            metalVials[i-1] = new MetalVial(
                contents,
                "Vial metálico",
                "Usado desde hace mucho tiempo por los alománticos para recargar sus reservas",
                i > metalVialNum,
                LoadSubSprite("Sprites/Menu/Datos/MetalVial","MetalVial_0"),
                LoadSubSprite("Sprites/Menu/Datos/MetalVial","MetalVial_1"),
                LoadSubSprite("Sprites/Menu/Datos/MetalVial","MetalVial_2"),
                this
            );
        }

        for(int i = 1; i <= rawMetals.Length; i++){
            rawMetals[i-1] = new RawMetal((Metal)i,DatabaseManager.Instance.GetInt($"SELECT raw_amount FROM metal_file WHERE file_id = {fileId} AND metal_id = {i}"));
        }
        
        for(int slot = 1; slot <= inventorySlots.Length; slot++){

            int inventorySlotMetalVial = DatabaseManager.Instance.GetInt($"SELECT slot_vial FROM inventory WHERE file_id = {fileId} AND slot = {slot}");
            if(inventorySlotMetalVial != 0){
                inventorySlots[slot-1] = metalVials[inventorySlotMetalVial-1];
                continue;
            }
            
            int inventorySlotLaudano = DatabaseManager.Instance.GetInt($"SELECT laudano FROM inventory WHERE file_id = {fileId} AND slot = {slot}");
            if(inventorySlotLaudano != 0){
                inventorySlots[slot-1] = laudano;
                continue;
            }
            
            int inventorySlotProj1 = DatabaseManager.Instance.GetInt($"SELECT proj_1 FROM inventory WHERE file_id = {fileId} AND slot = {slot}");
            if(inventorySlotProj1 != 0){
                inventorySlots[slot-1] = projectiles[0];
                continue;
            }
            
            int inventorySlotProj2 = DatabaseManager.Instance.GetInt($"SELECT proj_2 FROM inventory WHERE file_id = {fileId} AND slot = {slot}");
            if(inventorySlotProj2 != 0){
                inventorySlots[slot-1] = projectiles[1];
                continue;
            }
            
            int inventorySlotProj3 = DatabaseManager.Instance.GetInt($"SELECT proj_3 FROM inventory WHERE file_id = {fileId} AND slot = {slot}");
            if(inventorySlotProj3 != 0){
                inventorySlots[slot-1] = projectiles[2];
                continue;
            }
        }
    }

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

        baseWeight = 1f;
        weight = baseWeight;

        baseSpeed = 7f;
        speed = baseSpeed;


        baseBreathCapacity = 10f;
        breathCapacity = baseBreathCapacity;


        baseFireEntryTime = 5f;
        fireEntryTime = baseFireEntryTime;

        baseFireExitTime = 5f;
        fireExitTime = baseFireExitTime;

        baseFireHitTime = 2f;
        fireHitTime = baseFireHitTime;


        baseParryWindow = .5f;
        parryWindow = baseParryWindow;


        checkpoint = Convert.ToInt32(fileData["checkpoint_id"]);
        float posX = Convert.ToSingle(DatabaseManager.Instance.ExecuteScalar($"SELECT x_pos FROM checkpoint WHERE id = {checkpoint}"));
        float posY = Convert.ToSingle(DatabaseManager.Instance.ExecuteScalar($"SELECT y_pos FROM checkpoint WHERE id = {checkpoint}"));
        checkpointPosition = new Vector3(posX,posY,0);
        vials = Convert.ToInt32(fileData["vials"]);
        usedVials = 0;
        baseVialPower = Convert.ToInt32(fileData["vial_power"]);
        vialPower = baseVialPower;
        phase = fileData["phase"].ToString();
        phaseTime = Convert.ToInt32(fileData["phase_time"]);
        coins = Convert.ToInt32(fileData["coins"]);
        metalVialNum = Convert.ToInt32(fileData["metal_vials"]);
        laudano = new Laudano(
            Convert.ToInt32(fileData["laudano_bottles"]),
            "Láudano",
            "Vino blanco, azafrán, clavo, canela (y algunas sorpresitas) además de opio, crean un brebaje capaz de calmar las mentes más inquietas e intranquilas",
            Resources.LoadAll<Sprite>("Sprites/UI_Elements/HUD/Vial")[0]
        );
        projectiles[0] = new Projectile(
            Convert.ToInt32(DatabaseManager.Instance.GetInt($"SELECT proj_1 FROM inventory WHERE file_id = {fileId} AND proj_1 != 0")),
            "Flecha",
            "Se lanza",
            Resources.LoadAll<Sprite>("Sprites/Menu/Datos/Projectiles")[0],
            null
        );
        projectiles[1] = new Projectile(
            Convert.ToInt32(DatabaseManager.Instance.GetInt($"SELECT proj_2 FROM inventory WHERE file_id = {fileId} AND proj_2 != 0")),
            "Moneda",
            "Se lanza con acero",
            Resources.LoadAll<Sprite>("Sprites/Menu/Datos/Projectiles")[1],
            null
        );
        projectiles[2] = new Projectile(
            Convert.ToInt32(DatabaseManager.Instance.GetInt($"SELECT proj_3 FROM inventory WHERE file_id = {fileId} AND proj_3 != 0")),
            "Piedra",
            "Se lanza y hace ruido",
            Resources.LoadAll<Sprite>("Sprites/Menu/Datos/Projectiles")[2],
            null
        );

        eventNum = DatabaseManager.Instance.GetInt("SELECT COUNT(*) FROM event");
        List<int> retrievedAchievedEvent = DatabaseManager.Instance.GetIntListFromQuery($"SELECT event_id FROM achieved_event WHERE file_id = {fileId}");
        foreach(int i in retrievedAchievedEvent){
            Event e = new Event(i);
            achievedEvents.Add(e);
        }

        RetrieveInventory();

        SetMetals();
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

    public void EquipItem(Consumible consu){
        int slotIndex = -1;

        for (int i = 0; i < inventorySlots.Length; i++){
            if (inventorySlots[i] != null && inventorySlots[i].EqualsConsumible(consu)){
                slotIndex = i;
                break;
            }
        }

        if (slotIndex >= 0){
            inventorySlots[slotIndex] = null;
        } else {
            for (int i = 0; i < inventorySlots.Length; i++){
                if (inventorySlots[i] == null){
                    inventorySlots[i] = consu;
                    break;
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
        if( fm != null){
            fm.NextStatus();
        }
    }
    public int GetFeruStatusInSlot(int slot){
        FeruMetal fm = GetFeruMetalInSlot(slot);
        if(fm == null) return 0;
        return GetFeruMetalInSlot(slot).GetStatus();
    }

    public void ModifyFeruReservesInInfinited(){
        foreach(FeruMetal fm in feruMetals){
            if(fm == null) return;
            for(int i = 0; i < 3; i++){
                fm.UseMetalmind();
            }
        }
    }
    
    public void ExecuteFeruAndHemaUpdates(){
        weight = baseWeight;
        damage = baseDamage;
        vialPower = baseVialPower;
        weight = baseWeight;
        speed = baseSpeed;
        ITime = baseITime;

        breathCapacity = baseBreathCapacity;

        fireExitTime = baseFireExitTime;
        fireEntryTime = baseFireEntryTime;
        fireHitTime = baseFireHitTime;

        parryWindow = baseParryWindow;

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
                weight = (GetFeruMetalIfEquipped((int)Metal.IRON).IsStoring() ? baseWeight*.5f : GetFeruMetalIfEquipped((int)Metal.IRON).IsTapping() ? baseWeight*2f : baseWeight*1f);
                break;

            case Metal.STEEL:
                speed *= (GetFeruMetalIfEquipped((int)Metal.STEEL).IsStoring() ? .5f : GetFeruMetalIfEquipped((int)Metal.STEEL).IsTapping() ? 1.25f : 1f);
                break;

            case Metal.TIN:
                // DONE
                break;

            case Metal.PEWTER:
                damage += (GetFeruMetalIfEquipped((int)Metal.PEWTER).IsStoring() ? -baseDamage/4 : GetFeruMetalIfEquipped((int)Metal.PEWTER).IsTapping() ? baseDamage/2 : 0);
                break;

            case Metal.ZINC:
                ITime *= (GetFeruMetalIfEquipped((int)Metal.ZINC).IsStoring() ? .5f : GetFeruMetalIfEquipped((int)Metal.ZINC).IsTapping() ? 1.5f : 1f);
                break;

            case Metal.BRASS:
                fireHitTime *= (GetFeruMetalIfEquipped((int)Metal.BRASS).IsStoring() ? 1.5f : GetFeruMetalIfEquipped((int)Metal.BRASS).IsTapping() ? .5f : 1f);
                break;

            case Metal.COPPER:
                Debug.Log("FERUCHEMIC COPPER TODO");
                break;

            case Metal.BRONZE:
                parryWindow *= (GetFeruMetalIfEquipped((int)Metal.BRONZE).IsStoring() ? .5f : GetFeruMetalIfEquipped((int)Metal.BRONZE).IsTapping() ? 1.5f : 1f);
                break;

            case Metal.CADMIUM:
                breathCapacity *= (GetFeruMetalIfEquipped((int)Metal.CADMIUM).IsStoring() ? .5f : GetFeruMetalIfEquipped((int)Metal.CADMIUM).IsTapping() ? 1.5f : 1f);
                break;

            case Metal.BENDALLOY:
                vialPower += (int)(GetFeruMetalIfEquipped((int)Metal.ZINC).IsStoring() ? (int)-baseVialPower*.5f : GetFeruMetalIfEquipped((int)Metal.ZINC).IsTapping() ? (int)baseVialPower*.5f : 0f);
                break;

            case Metal.GOLD:
                maxHealth = (int)(GetFeruMetalIfEquipped((int)Metal.GOLD).IsStoring() ? baseMaxHealth/3 : baseMaxHealth);
                health = health > maxHealth ? maxHealth : health;
                break;

            case Metal.ELECTRUM:
                // DONE
                break;

            case Metal.CHROMIUM:
                // DONE
                break;

            case Metal.NICROSIL:
                // DONE
                break;

            case Metal.ALUMINIUM:
                // DONE
                break;

            case Metal.DURALUMIN:
                Debug.Log("FERUCHEMIC DURALUMIN TODO");
                break;

            case Metal.ATIUM:
                Debug.Log("FERUCHEMIC ATIUM TODO");
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
            case Metal.STEEL:
                speed *= (IsHemaMetalEquipped((int)Metal.STEEL) ? IsHemaMetalEquipped((int)Metal.DURALUMIN) ? 2f : 1.5f : 1f);
                break;

            case Metal.PEWTER:
                damage += (IsHemaMetalEquipped((int)Metal.STEEL) ? IsHemaMetalEquipped((int)Metal.DURALUMIN) ? baseDamage/2 : baseDamage/4 : 0);
                break;

            case Metal.ZINC:
                ITime *= (IsHemaMetalEquipped((int)Metal.ZINC) ? IsHemaMetalEquipped((int)Metal.DURALUMIN) ? 2f : 1.5f : 1f);
                break;

            case Metal.BRASS:
                fireEntryTime *= (IsHemaMetalEquipped((int)Metal.BRASS) ? IsHemaMetalEquipped((int)Metal.DURALUMIN) ? 1.5f : 1.25f : 1f);
                fireExitTime *= (IsHemaMetalEquipped((int)Metal.BRASS) ? IsHemaMetalEquipped((int)Metal.DURALUMIN) ? .5f : .75f : 1f);
                break;

            case Metal.CADMIUM:
                breathCapacity += (IsHemaMetalEquipped((int)Metal.CADMIUM) ? IsHemaMetalEquipped((int)Metal.DURALUMIN) ? baseBreathCapacity/2 : baseBreathCapacity/4 : 0);
                break;

            case Metal.BENDALLOY:
                vialPower += (IsHemaMetalEquipped((int)Metal.BENDALLOY) ? IsHemaMetalEquipped((int)Metal.DURALUMIN) ? baseVialPower/2 : baseVialPower/4 : 0);
                break;

            case Metal.GOLD:
                HemaGoldHeal(hm);
                break;

            case Metal.ALUMINIUM:
                foreach(AloMetal am in aloMetals){
                    if(am.IsBurning()) am.SetBurning(false);
                }
                break;
        }
    }

    public void Heal(){
        health++;
    }

    public void HemaGoldHeal(HemaMetal hm){
        if(health<maxHealth && hm.IsActive()){
            health+=2;
            hm.Deactivate();
        }
        if(health>=maxHealth)
            health = maxHealth;
    }

    public void Hurt(int hurtDamage){
        if(IsHemaMetalEquipped((int)Metal.ATIUM)){
            if(UnityEngine.Random.Range(1,101) < (IsHemaMetalEquipped((int)Metal.DURALUMIN) ? 20 : 10)){
                invulnerable = true;
                return;
            }
        }
        if(IsHemaMetalEquipped((int)Metal.ELECTRUM)){
            if(UnityEngine.Random.Range(1,101) < (IsHemaMetalEquipped((int)Metal.DURALUMIN) ? 20 : 10)){
                return;
            }
        }
        if(!invulnerable){
            int realHurt;

            AloMetal amPew = GetAloMetalIfEquipped((int)Metal.PEWTER);
            AloMetal amDur = GetAloMetalIfEquipped((int)Metal.DURALUMIN);
            FeruMetal fmEle = GetFeruMetalIfEquipped((int)Metal.ELECTRUM);

            if(amPew != null && amPew.IsBurning()){
                if(amDur != null && amDur.IsBurning()){
                    realHurt = hurtDamage/4;
                }else{
                    realHurt = hurtDamage/2;
                }
            }else{
                realHurt = hurtDamage;
            }

            if(fmEle != null){
                if(fmEle.IsStoring()){
                    if(UnityEngine.Random.Range(1,101) < 15){
                        realHurt += hurtDamage;
                    }
                }else if(fmEle.IsTapping()){
                    realHurt -= realHurt/2;
                }
            }
            health -= realHurt;
            if(health <= 0){
                alive = false;
            }
            invulnerable = true;
        }
    }

    public void EnvironmentHit(int hurtDamage){
        AloMetal amPew = GetAloMetalIfEquipped((int)Metal.PEWTER);
        health -= (amPew != null && amPew.IsBurning()) ? hurtDamage/4 : hurtDamage;
        if(health <= 0){
            alive = false;
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
        foreach(AloMetal aloMetal in aloMetals){
            aloMetal.SetBurning(false);
        }
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
        SaveMetalVials();
        SaveInventory();

        
        float posX = Convert.ToSingle(DatabaseManager.Instance.ExecuteScalar($"SELECT x_pos FROM checkpoint WHERE id = {checkpoint}"));
        float posY = Convert.ToSingle(DatabaseManager.Instance.ExecuteScalar($"SELECT y_pos FROM checkpoint WHERE id = {checkpoint}"));
        checkpointPosition = new Vector3(posX,posY,0);
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
            $"damage = {baseDamage}, "+
            $"vials = {vials}, "+
            $"vial_power = {baseVialPower}, "+
            $"phase = '{phase}', "+
            $"phase_time = {phaseTime}, "+
            $"coins = {coins} "+
            $"WHERE id = {fileId};"
        );
    }

    void SaveMetalVials(){
        DatabaseManager.Instance.ExecuteNonQuery($"DELETE FROM metal_vial_content WHERE file_id={fileId}");
        for(int i = 0; i < metalVials.Length; i++){
            foreach(MetalVialContent mvc in metalVials[i].content){
                DatabaseManager.Instance.ExecuteNonQuery($"INSERT INTO metal_vial_content (file_id,metal_id,vial_slot,amount) VALUES ({fileId},{(int)mvc.metal},{i+1},{mvc.amount})");
            }
        }
        foreach(RawMetal rm in rawMetals){
            DatabaseManager.Instance.ExecuteNonQuery($"UPDATE metal_file SET raw_amount = {rm.amount} WHERE file_id = {fileId} AND metal_id = {(int)rm.metal}");
        }
    }

    void SaveInventory(){
        int checkingSlot = 1;
        foreach(Consumible consu in inventorySlots){
            if(consu is MetalVial){
                Debug.Log(consu);
                for (int i = 0; i < metalVials.Length; i++){
                    if (metalVials[i] != null && metalVials[i].EqualsConsumible(consu)){
                        // slotIndex = i;
                        DatabaseManager.Instance.ExecuteNonQuery(
                            $"UPDATE inventory SET slot_vial = {i+1}, laudano = 0, proj_1 = 0, proj_2 = 0, proj_3 = 0 WHERE file_id = {fileId} AND slot = {checkingSlot}"
                        );
                        break;
                    }
                }
            }else if(consu is Laudano){
                DatabaseManager.Instance.ExecuteNonQuery(
                    $"UPDATE inventory SET slot_vial = 0, laudano = 1, proj_1 = 0, proj_2 = 0, proj_3 = 0 WHERE file_id = {fileId} AND slot = {checkingSlot}"
                );
            }else if(consu is Projectile){
                DatabaseManager.Instance.ExecuteNonQuery(
                    $"UPDATE inventory SET slot_vial = 0, laudano = 0, proj_1 = {(consu.GetName() == "Flecha" ? consu.GetStock() : 0)}, proj_2 = {(consu.GetName() == "Moneda" ? 1 : 0)}, proj_3 = {(consu.GetName() == "Piedra" ? consu.GetStock() : 0)} WHERE file_id = {fileId} AND slot = {checkingSlot}"
                );
            }else{

            }
            checkingSlot+=1;
        }
    }

    void SaveEventData(){
        foreach(Event e in achievedEvents){
            bool notAlreadyAchieved = DatabaseManager.Instance.GetInt($"SELECT COUNT(*) FROM achieved_event WHERE file_id={fileId} AND event_id={e.GetId()}") < 1;
            if(notAlreadyAchieved){
                DatabaseManager.Instance.ExecuteNonQuery(
                    $"INSERT INTO achieved_event (file_id,event_id) VALUES ({fileId},{e.GetId()})"
                );
            }
        }
    }

    public void AddCoins(int amount){
        float a = amount;

        if(GetFeruMetalIfEquipped((int)Metal.CHROMIUM) != null){
            a = a * (GetFeruMetalIfEquipped((int)Metal.CHROMIUM).IsStoring() ? .75f : GetFeruMetalIfEquipped((int)Metal.CHROMIUM).IsTapping() ? 1.25f : 1f);
        }
        if(GetHemaMetalIfEquipped((int)Metal.CHROMIUM) != null){
            a = a * (IsHemaMetalEquipped((int)Metal.CHROMIUM) ? IsHemaMetalEquipped((int)Metal.DURALUMIN) ? 1.5f : 1.25f : 1f);
        }

        this.coins+=(int)a;
    }

    public void Buy(int amount){
        coins -= amount;
    }

    public int GetFileId() => fileId;
    public int GetMaxHealth() => maxHealth;
    public int GetHealth() => health;
    public int GetCheckPoint() => checkpoint;
    public int GetDamage(){
        AloMetal amPew = GetAloMetalIfEquipped((int)Metal.PEWTER);
        AloMetal amDur = GetAloMetalIfEquipped((int)Metal.DURALUMIN);

        if (amPew != null && amPew.IsBurning()){
            if(amDur != null && amDur.IsBurning()){
                return (int)(damage*3f);
            }else{
                return (int)(damage*1.5f);
            }
        }else{
            return damage;
        }
    }
    public int GetVials() => vials;
    public int GetUsedVials() => usedVials;
    public int GetRemainingVials() => vials-usedVials;
    public int GetVialPower() => vialPower;
    public string GetPhase() => phase;
    public int GetPhaseTime() => phaseTime;
    public int GetCoins() => coins;
    public float GetWeight() => weight;
    public float GetSpeed() => speed;
    public float GetITime() => ITime;
    public bool IsInvulnerable() => invulnerable;
    public float GetBreathCapacity() => breathCapacity;
    public float GetFireEntryTime() => fireEntryTime;
    public float GetFireExitTime() => fireExitTime;
    public float GetFireHitTime() => fireHitTime;
    public float GetParryWindow() => parryWindow;
    public Laudano GetLaudano() => laudano;
    public Projectile[] GetProjectiles() => projectiles;
    public Consumible[] GetInventory() => inventorySlots;

    public int[] GetUnlockedAloMetals() => unlockedAloMetals;
    public int[] GetUnlockedFeruMetals() => unlockedFeruMetals;
    public int[] GetUnlockedHemaMetals() => unlockedHemaMetals;

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

    public MetalVial[] GetMetalVials() => metalVials;
    public RawMetal[] GetRawMetals() => rawMetals;

    public MetalVial GetMetalVialBySlot(int i) => metalVials[i-1];
    public RawMetal GetRawMetalByMetalId(int i) => rawMetals[i-1];

    public Vector3 GetCheckpointPosition() => checkpointPosition;


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
    public void AddLaudanoBottle() => laudano.AddStock();

    public void UnlockAloMetal(int metalId) => unlockedAloMetals[metalId-1] = 1;
    public void UnlockFeruMetal(int metalId) => unlockedFeruMetals[metalId-1] = 1;
    public void UnlockHemaMetal(int metalId) => unlockedHemaMetals[metalId-1] = 1;

    public void UnlockMetalVial(){
        metalVialNum+=1;
        GetMetalVialBySlot(metalVialNum-1).roto = false;
    }

    public void BuySomething(int price){
        if(coins-price >= 0)
            coins-=price;
    }

    public void BuyRawMetal(int metalId, int addedAmount, int price){
        rawMetals[metalId-1].amount += addedAmount;
        coins -= price;
    }

    public void AddRawMetalToMetalVial(int vialSlot, int metalId, int amount){
        metalVials[vialSlot].Add(new MetalVialContent((Metal)metalId,amount));
        SpendRawMetal(metalId,amount);
    }
    public void SpendRawMetal(int metalId, int spentAmount) => rawMetals[metalId-1].amount -= spentAmount;

    public void ConsumeVial(List<MetalVialContent> content) {
        foreach (MetalVialContent mvc in content) {
            foreach (AloMetal am in aloMetals) {
                if (am.GetMetal() == mvc.metal) {
                    am.Recharge(mvc.amount);
                    continue;
                }
            }
        }
    }

    public void SetProjectilePrefab(int projectile, GameObject pf){
        projectiles[projectile].SetPrefab(pf);
    }

    public override string ToString(){
        return $"PlayerData (File ID: {fileId}, Health: {health}/{maxHealth}, Damage: {damage}, Vials: {vials}, Vial Power: {vialPower}, Phase: {phase}, Phase Time: {phaseTime})";
    }
}

[System.Serializable]
public class RawMetal {
    public Metal metal;
    public int amount;

    public RawMetal(Metal m, int a){
        metal = m;
        amount = a;
    }

    public override string ToString(){
        return $"({metal}-{amount} RAW)";
    }
}
