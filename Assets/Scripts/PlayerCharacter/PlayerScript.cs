using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;

public class PlayerScript : MonoBehaviour{
    public GameObject playerLight;
    public GameObject[] bigMists;
    public GameObject smallMist;

    public bool inDoors;
    public bool outDoors;

    int coins;

    [Header("Movimiento")]
    [Tooltip("Velocidad del movimiento horizontal")]
    [Range(5,15)]
    [SerializeField] public float speed = 5;
    public bool facingRight = true;
    private Vector2 movementInput = Vector2.zero;

    [Header("Saltando")]
    [Tooltip("Altura del salto")]
    [Range(0,100)]
    [SerializeField] float thrust = 300;
    [SerializeField] bool grounded = true;
    bool jumped;
    bool attackGP;

    [Header("Atacando")]
    [SerializeField] GameObject Attack1_HB;
    bool attack_active;
    bool attack_pressed = false;


    
    [Header("Dash")]
    [Tooltip("Velocidad del Dash")]
    [SerializeField] float dashSpeedMultiplier = 3;
    [Tooltip("Duración del Dash")]
    [Range(.1f,.5f)]
    [SerializeField] float dashDuration = 0.2f;
    bool canDash = true;  
    bool isDashing = false;
    float dashTimeRemaining;

    [Header("Vida")]
    public int actual_lifes = 1;
    float targetTime;
    bool dead = false;
    [SerializeField] [Range(5,25)] float hurt_thrust;
    float short_term_thrust;

    Rigidbody2D rb;
    Animator playerAnimator;
    SpriteRenderer sr;

    
    bool doubleJumped;

    public bool interacting;
    bool inFuente = false;
    int fuenteActual;
    bool canExitFuente = false;


    [Header("Invulnerabilidad tras daño")]
    [SerializeField] private float invulnerabilityTime = 1.5f;
    private bool isInvulnerable = false;

    
    public GameObject cvMetalWheel;
    private bool selectingMetal = false;
    int selectedAloMetalSlot = 0;

    public Image[] slotsMetalWheel;
    public Image[] slotsMetalWheelMenu;
    public Sprite[] spritesMetales;


    public GameObject cvConsumibleWheel;
    private bool selectingConsumible = false;
    int selectedConsumibleSlot = 0;

    public Image[] slotsConsumibleWheel;
    public Image[] slotsConsumibleWheelMenu;
    public TextMeshProUGUI txtConsumibleRueda;

    private Vector2 selectionMetal = Vector2.zero;

    public GameObject musicObject;

    [Header("Metales")]
    string[] metales = {
        "Hierro",
        "Acero",
        "Estaño",
        "Peltre",
        "Zinc",
        "Latón",
        "Cobre",
        "Bronce",
        "Cadmio",
        "Bendaleo",
        "Oro",
        "Electro",
        "Cromo",
        "Nicrosil",
        "Aluminio",
        "Duralumín",
        "Atium",
    };
    private string[] descripcionMetalAlo = new string[] {
        // Hierro
        "Atrae objetos de metal al brumoso.\nSi el objeto es más pesado, el brumoso será atraído al metal.",

        // Acero
        "Repele objetos de metal al brumoso.\nSi el objeto es más pesado, el brumoso será repelido del metal.",

        // Estaño
        "Aumenta la percepción del brumoso.\nLa vista, oído y olfato se verán agudizados mientras se queme el metal.",

        // Peltre
        "Aumenta la fortaleza física del brumoso.\nLa fuerza, agilidad y resistencia se verán aumentados mientras se queme el metal.",

        // Zinc
        "Enardece las emociones del objetivo.\nTal es el efecto sobre las empciones que puede volver hostil al objetivo contra sus aliados.",

        // Latón
        "Apacigua las emociones del objetivo.\nDependiendo del objetivo, puede provocarle menos daño al brumoso o ignorarlo por completo.",

        // Cobre
        "Oculta la alomancia del brumoso.\nEsto permite ocultar al brumoso de los enemigos que puedan detectar la alomancia.",

        // Bronce
        "Muestra la alomancia de los brumosos cercanos.\nPermite detectar y conocer la alomancia de los brumosos cercanos.",

        // Cadmio
        "Crea una burbuja alrededor del brumoso que ralentiza el tiempo en su interior.\nLa percepción es que el tiempo fuera de la burbuja se acelera.",

        // Bendaleo
        "Crea una burbuja alrededor del brumoso que acelera el tiempo en su interior.\nLa percepción es que el tiempo fuera de la burbuja se ralentiza.",

        // Oro
        "Crea una proyección de quién podría haber sido el brumoso.\nTocar la \"sombra de oro\" causa efectos secundarios desagradables.",

        // Electro
        "Crea proyecciones de los posibles futuros cercanos del brumoso.\nAl conocer todas las posibilidades que puede tomar, contrarresta los efectos del atium.",

        // Cromo
        "Hace perder al objetivo las reservas alománticas.\nPara que haga efecto, tiene que quemarse el metal y tocar al objetivo.",

        // Nicrosil
        "Hace perder al objetivo las reservas alománticas de los metales que esté quemando en un estallido de poder.\nPara que haga efecto, tiene que quemarse el metal y tocar al objetivo.",

        // Aluminio
        "Hace perder al brumoso sus propias reservas alománticas.\nAl quemarlo, todas las reservas se perderán rápidamente.",

        // Duralumín
        "Hace perder al brumoso sus propias reservas alománticas de los metales que esté quemando en un estallido de poder.\nEste metal puede ser muy poderoso, pero peligroso dependiendo del metal que se queme.",

        // Atium
        "Permite al usuario ver unos segundos en el futuro, pudiendo prever qué pasará.\nUn metal muy poderoso, pero se consume con mucha facilidad."
    };
    public TextMeshProUGUI txtMetalRueda;
    

    public Sprite[] spritesBarrasMetales;
    public GameObject barraRueda;
    
    public List<Objeto> objetosDesbloqueados;

    LocuraManager locuraManager;
    PauseManager pauseManager;
    ParryManager parryManager;
    CadmiumBendalloyManager CyBManager;
    UnlockingManager unlockingManager;
    ShopManager shopManager;

    public bool nearAnyNPC = false;
    public GameObject nearNPC = null;
    public bool inNPC = false;
    public bool nextFrase = false;

    public PlayerData pd;

    public GameObject flechaProjectile;
    public GameObject monedaProjectile;
    public GameObject piedraProjectile;

    void Start(){
        pd = new PlayerData(1);

        pd.SetProjectilePrefab(0,flechaProjectile);
        pd.SetProjectilePrefab(1,monedaProjectile);
        pd.SetProjectilePrefab(2,piedraProjectile);

        cvMetalWheel.SetActive(false);
        cvConsumibleWheel.SetActive(false);

        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        locuraManager = FindObjectOfType<LocuraManager>();
        pauseManager = FindObjectOfType<PauseManager>();
        parryManager = FindObjectOfType<ParryManager>();
        CyBManager = FindObjectOfType<CadmiumBendalloyManager>();
        unlockingManager = FindObjectOfType<UnlockingManager>();
        shopManager = FindObjectOfType<ShopManager>();

        Vector3 checkpointPosition = pd.GetCheckpointPosition();
        transform.position = checkpointPosition;
        checkpointPosition.z = camera.position.z;
        // checkpointPosition.y += 3;
        camera.position = checkpointPosition;
        StartCoroutine(UpdateAmountEveryHalfSecond());
        StartCoroutine(UpdateAloMetals());
    }

    public void OnFeruUp(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused && pd.GetFeruMetalInSlot(1)!=null && !pressingQ){
            pd.ChangeFeruMetalStatus(1);
        }
    }
    public void OnFeruRight(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused && pd.GetFeruMetalInSlot(2)!=null && !pressingQ){
            pd.ChangeFeruMetalStatus(2);
        }
    }
    public void OnFeruDown(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused && pd.GetFeruMetalInSlot(3)!=null && !pressingQ){
            pd.ChangeFeruMetalStatus(3);
        }
    }
    public void OnFeruLeft(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused && pd.GetFeruMetalInSlot(4)!=null && !pressingQ){
            pd.ChangeFeruMetalStatus(4);
        }
    }

    IEnumerator UpdateAmountEveryHalfSecond() {
        while (true) {
            for (int i = 1; i <= pd.GetFeruSlots().Length; i++) {        
                pd.GetFeruMetalInSlot(i)?.UseMetalmind();
            }
            for (int i = 1; i <= pd.GetAloSlots().Length; i++) {
                AloMetal amAlu = pd.GetAloMetalIfEquipped((int)Metal.ALUMINIUM);
                if(amAlu != null && amAlu.IsBurning()){
                    foreach(AloMetal am in pd.GetAloMetals()){
                        if(am.GetMetal() != Metal.ALUMINIUM){
                            am.AluminiumBurn();
                        }else{
                            pd.GetAloMetalInSlot(i)?.Burn(100);
                        }
                    }
                }
            }
            FeruMetal fmGol = pd.GetFeruMetalIfEquipped((int)Metal.GOLD);
            if(fmGol != null && fmGol.IsTapping()){
                pd.Heal();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator UpdateAloMetals() {
        while (true) {
            AloMetal amAlu = pd.GetAloMetalIfEquipped((int)Metal.ALUMINIUM);
            AloMetal amDur = pd.GetAloMetalIfEquipped((int)Metal.DURALUMIN);
            for (int i = 1; i <= pd.GetAloSlots().Length; i++) {
                if(amAlu != null && amAlu.IsBurning()){
                    foreach(AloMetal am in pd.GetAloMetals()){
                        if(am.GetMetal() != Metal.ALUMINIUM){
                            am.AluminiumBurn();
                        }else{
                            pd.GetAloMetalInSlot(i)?.Burn(10);
                        }
                    }
                }else{
                    if (amDur != null && amDur.IsBurning()){
                        pd.GetAloMetalInSlot(i)?.Burn(30);
                    }else{
                        pd.GetAloMetalInSlot(i)?.Burn();
                    }
                }
            }
            FeruMetal fmNic = pd.GetFeruMetalIfEquipped((int)Metal.NICROSIL);
            HemaMetal hmNic = pd.GetHemaMetalIfEquipped((int)Metal.NICROSIL);
            HemaMetal hmDur = pd.GetHemaMetalIfEquipped((int)Metal.DURALUMIN);
            float multNic = 1f;

            if(fmNic != null){
                multNic = fmNic.IsStoring() ? .5f : fmNic.IsTapping() ? 1.5f : 1f;
            }
            if(hmNic != null && hmDur != null){
                multNic *= 1.4f;
            }else if(hmNic != null){
                multNic *= 1.2f;
            }
            float waitTime = (((amDur != null && amDur.IsBurning()) || (amAlu != null && amAlu.IsBurning())) ? .2f : 1f) * multNic;
            yield return new WaitForSeconds(waitTime);
        }
    }

    void TurnOffUnusedAloMetal(){
        foreach(AloMetal am in pd.GetAloMetals()){
            if(pd.GetAloMetalIfEquipped((int)am.GetMetal()) == null){
                am.SetBurning(false);
            }
        }
    }

    private Coroutine hemaGoldCoroutine;

    void SetActiveMists(float opacity){
        foreach(GameObject bigMist in bigMists){
            // bigMist.gameObject.SetActive(state);
            bigMist.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f, inDoors ? 0f : outDoors ? opacity/2 : opacity);
        }
    }

    void FixedUpdate(){
        playerLight.transform.position = transform.position;
        Light2D pl = playerLight.GetComponent<Light2D>();
        // pl.pointLightOuterRadius = 14;
        // pl.pointLightInnerRadius = 4;
        FeruMetal fmTin = pd.GetFeruMetalIfEquipped((int)Metal.TIN);
        AloMetal amTin = pd.GetAloMetalIfEquipped((int)Metal.TIN);
        if(amTin != null && amTin.IsBurning()){
            if(fmTin != null && fmTin.IsStoring()){
                // QUEMANDO Y GUARDANDO
                SetActiveMists(.8f);

            }else if(fmTin != null && fmTin.IsTapping()){
                // QUEMANDO Y DECANTANDO
                SetActiveMists(.3f);

            }else{
                // QUEMANDO
                SetActiveMists(.4f);
            }
        }else{
            if(fmTin != null && fmTin.IsStoring()){
                SetActiveMists(1f);
            }else if(fmTin != null && fmTin.IsTapping()){
                SetActiveMists(.6f);
            }else{
                SetActiveMists(.8f);
            }
        }

        if(short_term_thrust != 0){short_term_thrust *= 0.8f;}
        int map_active = PlayerPrefs.GetInt("map_active");

        if(map_active==0){
            if (isDashing){
                Dash();
            }else{
                playerAnimator.SetBool("Grounded", grounded);
                playerAnimator.SetFloat("Velocity", rb.velocity.y);

                if(!dead){
                    rb.mass = pd.GetWeight();
                    if(!unlockingManager.showingPanel){
                        Fuente();
                        Move();
                        Jump();
                        Attack1();
                    }

                    ChokeByToxicMist();
                    BurnWithFire();

                    pd.ExecuteFeruAndHemaUpdates();
                    if(pd.IsHemaMetalEquipped((int)Metal.GOLD)){
                        if(hemaGoldCoroutine == null){
                            hemaGoldCoroutine = StartCoroutine(ResetHemaGoldCooldown());
                        }
                    } else {
                        if(hemaGoldCoroutine != null){
                            StopCoroutine(hemaGoldCoroutine);
                            hemaGoldCoroutine = null;
                        }
                    }
                }

                if(pd.IsInvulnerable() && !activeIFrames){
                    StartCoroutine(ResetVulnerability());
                }

                if(pd.IsInvulnerable()){
                    sr.color = Color.red;
                }else if(parryManager.canParry){
                    sr.color = Color.cyan;
                }else if(parryManager.inCooldown){
                    sr.color = Color.gray;
                }else if(toxicHit){
                    sr.color = Color.green;
                }else if(inToxicMist){
                    sr.color = new Color(1f, 1-(breathUsed/pd.GetBreathCapacity()), 1f, 1f );
                }else if(burned){
                    sr.color = new Color(255f/255f, 153f/255, 43f/255f, 1f);
                }else if(inFire){
                    sr.color = new Color(1f, 1-(inFireTime/pd.GetFireEntryTime()), 1-(inFireTime/pd.GetFireEntryTime()), 1f);
                }else{
                    sr.color = Color.white;
                }

                if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Vin_Attacking") && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.75f && playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f){
                    attack_active = true;
                    Attack1_HB.SetActive(true);
                }else{
                    attack_active = false;
                    Attack1_HB.SetActive(false);
                }
            }
        }
    }

    bool activeIFrames = false;

    private IEnumerator ResetVulnerability(){
        activeIFrames = true;
        float invulnerabilityTime = pd.GetITime();

        if(pd.IsFeruMetalEquipped((int)Metal.ZINC)){
            if(pd.GetFeruMetalIfEquipped((int)Metal.ZINC).GetStatus() == 1){
                invulnerabilityTime *= .5f;
            }
            if(pd.GetFeruMetalIfEquipped((int)Metal.ZINC).GetStatus() == -1){
                invulnerabilityTime *= 1.5f;
            }
        }

        yield return new WaitForSecondsRealtime(invulnerabilityTime);
        pd.MakeVulnerable();
        activeIFrames = false;
    }
    private IEnumerator ResetHemaGoldCooldown(){
        yield return new WaitForSecondsRealtime(pd.IsHemaMetalEquipped((int)Metal.ELECTRUM) ? 7f : 15f);
        HemaMetal hmGol = pd.GetHemaMetalIfEquipped((int)Metal.GOLD);
        if( hmGol != null ){
            hmGol.Activate();
        }
        StartCoroutine(ResetHemaGoldCooldown());
    }

    void Update(){
        SetMetalWheel();
        SetConsumibleWheel();
        SetObjective();

        TurnOffUnusedAloMetal();
        InUpdate();
    }

    private void SetMetalWheel(){
        for(int i = 1; i <= pd.GetAloSlots().Length; i++){
            AloMetal am = pd.GetAloMetalInSlot(i);
            int idMetal = 0;
            if(am != null){
                idMetal = (int)am.GetMetal();
            }

            if(idMetal <= 0){
                Color c = slotsMetalWheel[i-1].color;
                Color cM = slotsMetalWheelMenu[i-1].color;
                c.a = 0f;
                cM.a = 0f;
                slotsMetalWheel[i-1].color = c;
                slotsMetalWheelMenu[i-1].color = cM;
            }else{
                Color c = slotsMetalWheel[i-1].color;
                Color cM = slotsMetalWheelMenu[i-1].color;
                c.a = 1f;
                cM.a = 1f;
                slotsMetalWheel[i-1].color = c;
                slotsMetalWheelMenu[i-1].color = cM;
                slotsMetalWheel[i-1].sprite = spritesMetales[idMetal - 1];
                slotsMetalWheelMenu[i-1].sprite = spritesMetales[idMetal - 1];
                if(i == selectedAloMetalSlot){
                    if(pd.GetHemaMetalIfEquipped((int)Metal.ALUMINIUM) != null){
                        slotsMetalWheel[i-1].color = Color.black;
                        slotsMetalWheelMenu[i-1].color = Color.black;
                    }else if(am.GetAmount() <= 0){
                        slotsMetalWheel[i-1].color = Color.gray;
                        slotsMetalWheelMenu[i-1].color = Color.gray;
                    }else if(!am.IsBurning()){
                        slotsMetalWheel[i-1].color = Color.red;
                        slotsMetalWheelMenu[i-1].color = Color.white;
                    }else{
                        slotsMetalWheel[i-1].color = new Color(1f, .5f, .5f, 1f);
                        slotsMetalWheelMenu[i-1].color = new Color(1f, .5f, 0f, 1f);
                    }
                    Image cantMetal = barraRueda.transform.GetChild(0).GetComponent<Image>();
                    Image capMetal = barraRueda.transform.GetChild(1).GetComponent<Image>();

                    float cant = (float)am.GetAmount();
                    float cap = (float)am.GetCapacity();
                    
                    if(selectedAloMetalSlot < 1){
                        capMetal.gameObject.SetActive(false);
                        cantMetal.gameObject.SetActive(false);
                    }else{
                        capMetal.gameObject.SetActive(true);
                        cantMetal.gameObject.SetActive(true);
                        float fillAmount = cant / cap;
                        capMetal.sprite = spritesBarrasMetales[idMetal - 1];
                        cantMetal.fillAmount = fillAmount;
                    }

                    txtMetalRueda.text = $"<b>{metales[idMetal-1]}</b><br>" +
                     $"<size=16>{descripcionMetalAlo[idMetal-1]}</size><br>"+
                     $"<size=16>{cant} - {cap}</size>";
                }else{
                    if(pd.GetHemaMetalIfEquipped((int)Metal.ALUMINIUM) != null){
                        slotsMetalWheel[i-1].color = Color.black;
                        slotsMetalWheelMenu[i-1].color = Color.black;
                    }else if(am.GetAmount() <= 0){
                        slotsMetalWheel[i-1].color = Color.gray;
                        slotsMetalWheelMenu[i-1].color = Color.gray;
                    }else if(!am.IsBurning()){
                        slotsMetalWheel[i-1].color = Color.white;
                        slotsMetalWheelMenu[i-1].color = Color.white;
                    }else{
                        slotsMetalWheel[i-1].color = new Color(1f, .5f, 0f, 1f);
                        slotsMetalWheelMenu[i-1].color = new Color(1f, .5f, 0f, 1f);
                    }
                }
            }
        }
    }

    public void OnOpenMetalWheel(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused){
            selectingMetal = true;
            cvMetalWheel.SetActive(true);
            Time.timeScale = 0.2f;
            musicObject.GetComponent<AudioSource>().pitch = 0.7f;
        }

        if (context.canceled){
            selectingMetal = false;
            cvMetalWheel.SetActive(false);
            Time.timeScale = 1f;
            musicObject.GetComponent<AudioSource>().pitch = 1f;
        }
    }

    private void SetConsumibleWheel(){
        Consumible[] inventory = pd.GetInventory();

        for (int i = 0; i < inventory.Length; i++){
            Consumible cons = inventory[i];

            if (cons == null){
                // Slot vacío
                Color c = slotsConsumibleWheel[i].color;
                Color cM = slotsConsumibleWheelMenu[i].color;
                c.a = 0f;
                cM.a = 0f;
                slotsConsumibleWheel[i].color = c;
                slotsConsumibleWheelMenu[i].color = cM;
            } else {
                // Slot con consumible
                Sprite sprite = cons.GetSprite();

                slotsConsumibleWheel[i].sprite = sprite;
                slotsConsumibleWheelMenu[i].sprite = sprite;

                Color normalColor = Color.white;
                Color selectedColor = new Color(1f, 0.75f, 0.25f, 1f); // dorado suave

                if (i + 1 == selectedConsumibleSlot){
                    slotsConsumibleWheel[i].color = selectedColor;
                    slotsConsumibleWheelMenu[i].color = selectedColor;

                    txtConsumibleRueda.text = $"<b>{cons.GetName()}</b><br>" +
                                            $"<size=16>{cons.GetDescription()}</size>";
                } else {
                    slotsConsumibleWheel[i].color = normalColor;
                    slotsConsumibleWheelMenu[i].color = normalColor;
                }
            }
        }
    }

    public void OnConsume(InputAction.CallbackContext context){
        if(!selectingConsumible && !selectingMetal && !pauseManager.isPaused && context.action.triggered){
            pd.GetInventory()[selectedConsumibleSlot-1].Consume();
        }
    }

    public void OnOpenConsumibleWheel(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused){
            selectingConsumible = true;
            cvConsumibleWheel.SetActive(true);
            Time.timeScale = 0.2f;
            musicObject.GetComponent<AudioSource>().pitch = 0.7f;
        }

        if (context.canceled){
            selectingConsumible = false;
            cvConsumibleWheel.SetActive(false);
            Time.timeScale = 1f;
            musicObject.GetComponent<AudioSource>().pitch = 1f;
        }
    }

    public void OnMetalSelection(InputAction.CallbackContext context){
        if(selectingMetal){
            selectionMetal = Vector2.zero;
            selectionMetal = context.ReadValue<Vector2>();
            float X = selectionMetal.x;
            float Y = selectionMetal.y;

            if(-.3<X && X<.3 && .9<Y && Y<=1){
                selectedAloMetalSlot = 1;
            }else if(.3<X && X<.9 && .3<Y && Y<.9){
                selectedAloMetalSlot = 2;
            }else if(.9<X && X<=1 && -.3<Y && Y<.3){
                selectedAloMetalSlot = 3;
            }else if(.3<X && X<.9 && -.9<Y && Y<-.3){
                selectedAloMetalSlot = 4;
            }else if(-.3<X && X<.3 && -1<=Y && Y<-.9){
                selectedAloMetalSlot = 5;
            }else if(-.9<X && X<-.3 && -.9<Y && Y<-.3){
                selectedAloMetalSlot = 6;
            }else if(-1<=X && X<-.9 && -.3<Y && Y<.3){
                selectedAloMetalSlot = 7;
            }else if(-.9<X && X<-.3 && .3<Y && Y<.9){
                selectedAloMetalSlot = 8;
            }
            
        }else if(selectingConsumible){
            selectionMetal = Vector2.zero;
            selectionMetal = context.ReadValue<Vector2>();
            float X = selectionMetal.x;
            float Y = selectionMetal.y;

            if(-.3<X && X<.3 && .9<Y && Y<=1){
                selectedConsumibleSlot = 1;
            }else if(.3<X && X<.9 && .3<Y && Y<.9){
                selectedConsumibleSlot = 2;
            }else if(.9<X && X<=1 && -.3<Y && Y<.3){
                selectedConsumibleSlot = 3;
            }else if(.3<X && X<.9 && -.9<Y && Y<-.3){
                selectedConsumibleSlot = 4;
            }else if(-.3<X && X<.3 && -1<=Y && Y<-.9){
                selectedConsumibleSlot = 5;
            }else if(-.9<X && X<-.3 && -.9<Y && Y<-.3){
                selectedConsumibleSlot = 6;
            }else if(-1<=X && X<-.9 && -.3<Y && Y<.3){
                selectedConsumibleSlot = 7;
            }else if(-.9<X && X<-.3 && .3<Y && Y<.9){
                selectedConsumibleSlot = 8;
            }
        }
    }

    void SelectAloMetal(){
        if(pd.GetHemaMetalIfEquipped((int)Metal.ALUMINIUM) != null) return;
        if(pd.GetAloMetalInSlot(selectedAloMetalSlot).GetAmount() <= 0) return;
        pd.ChangeAloMetalBurning(selectedAloMetalSlot);
    }

    void SelectConsumible(){
        Debug.Log($"ELEGIDO EL CONSUMIBLE Nº: {selectedConsumibleSlot}");
    }

    public void OnMove(InputAction.CallbackContext context){
        if(selectingMetal || pressingQ){
            return;
        }
        if(tping){
            movementInput.x = 0;
            movementInput.y = 0;
            return;
        }
        movementInput = context.ReadValue<Vector2>();
    }

    public void GetHurt(int damage){
        pd.Hurt(damage);
        float thrust = 10f * (facingRight ? -1f : 1f);
        FeruMetal fmIro = pd.GetFeruMetalIfEquipped((int)Metal.IRON);
        HemaMetal hmIro = pd.GetHemaMetalIfEquipped((int)Metal.IRON);
        HemaMetal hmDur = pd.GetHemaMetalIfEquipped((int)Metal.DURALUMIN);
        if(fmIro != null){
            if(fmIro.IsStoring()){
                thrust*=1.3f;
            }else if(fmIro.IsTapping()){
                thrust*=.7f;
            }
        }
        if(hmIro != null){
            if(hmDur != null){
                thrust*=.4f;
            }else{
                thrust*=.8f;
            }
        }
        short_term_thrust = thrust;
        // Vector2 pushDirection = new Vector2(thrust, 0f);
        // playerScript.GetRB().AddForce(pushDirection, ForceMode2D.Impulse);
    }
    void Move(){
        if(inNPC) return;
        if(pressingQ)return;
        rb.velocity = new Vector2(short_term_thrust*2 + movementInput.x * pd.GetSpeed() * 1, rb.velocity.y+(short_term_thrust != 0 ? .1f : 0f));

        if(Mathf.Abs(movementInput.x)>0.01f){
            playerAnimator.SetBool("Running",true);
        }else{
            playerAnimator.SetBool("Running",false);
        }

        if(movementInput.x>0.01f && !facingRight){
            transform.Rotate(0,180,0);
            facingRight = true;
        }
        if(movementInput.x<-0.01f && facingRight){
            transform.Rotate(0,180,0);
            facingRight = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context){
        if(inNPC) return;
        if(context.started && selectingMetal){
            SelectAloMetal();
            return;
        }
        if(context.started && selectingConsumible){
            SelectConsumible();
            return;
        }
        if (context.started && !pauseManager.isPaused) {
            if (grounded){
                jumped = true;
            } else if (coins > 0){
                doubleJumped = true;
                coins--;
            }
        }
    }
    void Jump(){
        if (jumped && grounded){
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * thrust * 100);
            grounded = false;
            jumped = false;
            playerAnimator.SetTrigger("Jumping");
        }
        else if (doubleJumped && !grounded){
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * thrust * 100);
            doubleJumped = false;
            playerAnimator.SetTrigger("Jumping");
        }
    }

    public void OnAttack(InputAction.CallbackContext context){
        attackGP = context.action.triggered;
    }
    void Attack1(){
        if(attackGP){
            if(!attack_pressed && !attack_active){
                attack_pressed = true;
                playerAnimator.SetTrigger("AttackTrigger");
                attack_active = true;
            }
        }else{
            attack_pressed = false;
        }
    }

    public void OnDash(InputAction.CallbackContext context){
        // int peltre = PlayerPrefs.GetInt("Peltre");
        // Debug.Log("Peltre: "+peltre);
        if (context.performed && canDash && !pressingQ){
            // PlayerPrefs.SetInt("Peltre",peltre-3);
            StartDash();
        }
    }

    public void OnHeal(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused){
            Heal();
        }
    }

    private void Heal(){
        pd.UseVial();
    }

    public void OnEnterFuente(InputAction.CallbackContext context){
        if(!selectingMetal)
            interacting = context.action.triggered;
    }

    public GameObject[] hidableHUD;
    public void ShowHUD(){
        foreach(GameObject gm in hidableHUD){
            gm.gameObject.SetActive(true);
        }
    }
    public void HideHUD(){
        foreach(GameObject gm in hidableHUD){
            gm.gameObject.SetActive(false);
        }
    }

    void Fuente() {
        if (nearAnyNPC && interacting){
            inNPC = true;
            var npcBehaviour = nearNPC.GetComponent<NPCBehaviour>();
            if (npcBehaviour != null){
                npcBehaviour.StartDialogue();
            }
            speed = 0;
        }
        if (nearShop && interacting){
            inShop = true;
            var gotShop = shop.GetComponent<BuyingSpot>();
            if (gotShop != null){
                // Debug.Log($"VÁMONOS DE COMPRASSS {shop}");
                // Debug.Log($"VÁMONOS DE COMPRASSS {gotShop}");
                shopManager.OpenPanel(gotShop.buyingThing);
            }
            speed = 0;
        }
        if (interacting && inFuente) {
            pd.EnterFuente();
            burned = false;
            inFireTime = 0f;
            speed = 0;
            
            canExitFuente = false;
            interacting = false;
            StartCoroutine(EnableExitFuente(1f));
            int visited = (int)(long)DatabaseManager.Instance.ExecuteScalar("SELECT COUNT(*) FROM visited_checkpoint WHERE file_id = 1 AND checkpoint_id = "+fuenteActual+";");
            if(visited == 0){
                DatabaseManager.Instance.ExecuteNonQuery("INSERT INTO visited_checkpoint (file_id, checkpoint_id) VALUES (1, "+fuenteActual+");");
            }
            DatabaseManager.Instance.ExecuteNonQuery($"UPDATE file SET checkpoint_id = {fuenteActual} WHERE id = 1;");
        }

        if (canExitFuente && (movementInput.x != 0 || jumped || doubleJumped) && inFuente) {
            inFuente = false;
            speed = 5;
            //StopAllCoroutines();
            canExitFuente = false;
        }
    }


    IEnumerator EnableExitFuente(float waitTime) {
        yield return new WaitForSeconds(waitTime);
        canExitFuente = true;
    }

    void StartDash(){

        rb.gravityScale = 0;
        isDashing = true;
        canDash = false;
        dashTimeRemaining = dashDuration;
        rb.velocity = new Vector2(movementInput.x * speed * dashSpeedMultiplier, 0);
    }

    void Dash(){
        dashTimeRemaining -= Time.deltaTime;

        if (dashTimeRemaining <= 0){
            isDashing = false;
            rb.gravityScale = 2;
            rb.velocity = new Vector2(movementInput.x * speed , 0);
        }
    }

    IEnumerator Invulnerability() {
        isInvulnerable = true;
        sr.color = new Color(1, 0, 0, 0.5f);
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
        sr.color = Color.white;
    }

    bool inToxicMist = false;
    float breathUsed = 0f;
    bool toxicHit = false;

    void ChokeByToxicMist(){
        if(inToxicMist){
            breathUsed += Time.deltaTime;
            Debug.Log($"breathUsed: {breathUsed}");
            if(breathUsed >= pd.GetBreathCapacity()){
                breathUsed = 0f;
                pd.EnvironmentHit(15);
                toxicHit = true;
                StartCoroutine(ToxicHit());
            }
        }else{
            breathUsed -= Time.deltaTime;
            if(breathUsed <= 0f){
                breathUsed = 0f;
            }
        }
    }

    IEnumerator ToxicHit(){
        yield return new WaitForSeconds(.5f);
        toxicHit = false;
    }


    bool inFire = false;
    float inFireTime = 0f;
    float exitFireTime = 0f;
    bool burned = false;
    float nextFireHit;

    void BurnWithFire(){
        if(inFire){
            inFireTime += Time.deltaTime;
            if(inFireTime >= pd.GetFireEntryTime()){
                inFireTime = pd.GetFireEntryTime();
                if(!burned){
                    burned = true;
                    StartCoroutine(GetHitByFire());
                }
            }
        }else{
            if(burned){
                exitFireTime += Time.deltaTime;
                if(exitFireTime >= pd.GetFireExitTime()){
                    burned = false;
                    inFireTime = 0f;
                    exitFireTime = 0f;
                }
            }else{
                inFireTime -= Time.deltaTime;
                if(inFireTime <= 0){
                    inFireTime = 0;
                }
            }
        }
    }

    IEnumerator GetHitByFire(){
        if(burned){
            pd.EnvironmentHit(8);
            yield return new WaitForSeconds(pd.GetFireHitTime());
            StartCoroutine(GetHitByFire());
        }
    }

    void OnTriggerStay2D(Collider2D other){
        if (other.tag == "Floor"){
            grounded = true;
            canDash = true;
        }

        if (other.tag == "ToxicMist"){
            inToxicMist = true;
        }
        if (other.tag == "Fire"){
            inFire = true;
        }
    }

    bool nearShop;
    bool inShop;
    GameObject shop;

    void OnTriggerEnter2D(Collider2D other){

        if (other.tag == "Fuente" && !dead) {
            inFuente = true;
            string[] nombrePartes = other.name.Split(" ");
            if (nombrePartes.Length > 1) {
                int fuenteID;
                if (int.TryParse(nombrePartes[1], out fuenteID)) {
                    fuenteActual = fuenteID;
                } else {
                    Debug.LogWarning("El ID de la fuente no es un número válido.");
                }
            }
        }

        if (other.CompareTag("NPC")){
            nearAnyNPC = true;
            nearNPC = other.gameObject;
            if (nearNPC != null) {
                var npcBehaviour = nearNPC.GetComponent<NPCBehaviour>();
                if (npcBehaviour != null){
                } else {
                }
            }
        }

        if (other.CompareTag("BuyingSpot")){
            nearShop = true;
            shop = other.gameObject;
            Debug.Log("EN UNA TIENDITA");
        }

        if (other.tag == "Instant_Death"){
            actual_lifes = 0;
            dead = true;
            sr.color = Color.white;
            playerAnimator.SetTrigger("DeadTrigger");
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.tag=="Fuente"){
            inFuente = false;
            speed = 5;
        }

        if (other.CompareTag("NPC")){
            nearAnyNPC = false;
            nearNPC = null;
            inNPC = false;
        }
        if (other.CompareTag("BuyingSpot")){
            nearShop = false;
            shop = null;
            inShop = false;
        }

        if (other.CompareTag("ToxicMist")){
            inToxicMist = false;
        }
        
        if (other.CompareTag("Fire")){
            inFire = false;
        }

        if(CyBManager.inCadBubble && other.tag=="CadmiumBubble"){
            CyBManager.inCadBubble = false;

            AloMetal amCad = pd.GetAloMetalIfEquipped((int)Metal.CADMIUM);
            AloMetal amDur = pd.GetAloMetalIfEquipped((int)Metal.DURALUMIN);

            if(amCad != null && amCad.IsBurning()){
                if(amDur != null && amDur.IsBurning()) return;
                pd.GetAloMetalIfEquipped((int)Metal.CADMIUM).SetBurning(false);
            }
        }
        
        if(CyBManager.inBenBubble && other.tag == "BendalloyBubble"){
            CyBManager.inBenBubble = false;

            AloMetal amBen = pd.GetAloMetalIfEquipped((int)Metal.BENDALLOY);
            AloMetal amDur = pd.GetAloMetalIfEquipped((int)Metal.DURALUMIN);

            if(amBen != null && amBen.IsBurning()){
                if(amDur != null && amDur.IsBurning()) return;
                pd.GetAloMetalIfEquipped((int)Metal.BENDALLOY).SetBurning(false);
            }
        }
    }

    public bool hideUnlockingPanel;
    public void OnNextFrase(InputAction.CallbackContext context){
        if(nearAnyNPC && inNPC && context.action.triggered){
            nextFrase = true;
        }else{
            nextFrase = false;
        }

        if(unlockingManager.canHidePanel){
            hideUnlockingPanel = true;
        }
    }

    public string GetNombreMetal(int metalId){
        return DatabaseManager.Instance.GetString(
            $"SELECT name FROM metal_file WHERE file_id = 1 AND metal_id = {metalId};"
        );
    }
    public int GetIdMetal(string nombreMetal){
        // Debug.Log($"nombreMetal: {nombreMetal}");
        return DatabaseManager.Instance.GetInt($"SELECT id FROM metals WHERE name = '{nombreMetal}';");
    }

    void OnEnable() {
        GameEvents.OnNPCSpokenTo += RegistrarDialogoConNPC;
    }

    void OnDisable() {
        GameEvents.OnNPCSpokenTo -= RegistrarDialogoConNPC;
    }

    void RegistrarDialogoConNPC(string eventName){
        Debug.Log($"evento disparado: {eventName}");

        string nombreEvento = $"{eventName}";

        pd.AddEvent(eventName);
    }

    
    bool pressingQ = false;
    public List<GameObject> nearMetalObjects;
    public List<GameObject> nearEmotionObjects;
    int selectedMetalObjective = 0;
    int selectedEmotionObjective = 0;
    bool canSelectObjective = true;

    public void OnQPressed(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused){
            pressingQ = true;
        }

        if (context.canceled){
            pressingQ = false;
        }
    }

    public void OnQRight(InputAction.CallbackContext context){
        if(pressingQ && nearMetalObjects.Count > 0 || nearEmotionObjects.Count > 0 && canSelectObjective){

            selectedMetalObjective++;
            selectedEmotionObjective++;

            if(selectedMetalObjective > nearMetalObjects.Count-1){
                selectedMetalObjective = 0;
            }
            if(selectedEmotionObjective > nearEmotionObjects.Count-1){
                selectedEmotionObjective = 0;
            }

            canSelectObjective = false;
            StartCoroutine(ObjectiveCooldown());
        }
    }
    public void OnQLeft(InputAction.CallbackContext context){
        if(pressingQ && nearMetalObjects.Count > 0 || nearEmotionObjects.Count > 0 && canSelectObjective){

            selectedMetalObjective--;
            selectedEmotionObjective--;

            if(selectedMetalObjective < 0){
                selectedMetalObjective = nearMetalObjects.Count-1;
            }
            if(selectedEmotionObjective < 0){
                selectedEmotionObjective = nearEmotionObjects.Count-1;
            }

            canSelectObjective = false;
            StartCoroutine(ObjectiveCooldown());
        }
    }

    IEnumerator ObjectiveCooldown(){
        if(!canSelectObjective){
            yield return new WaitForSeconds(.2f);
            canSelectObjective = true;
        }
    }

    void SetObjective(){
        if(!pressingQ) return;
        if(selectedMetalObjective > nearMetalObjects.Count-1){
            selectedMetalObjective = nearMetalObjects.Count-1;
        }
        if(selectedEmotionObjective > nearEmotionObjects.Count-1){
            selectedEmotionObjective = nearEmotionObjects.Count-1;
        }

        int pos = 0;
        foreach(GameObject gm in nearMetalObjects){
            MetalObject mo = gm.GetComponent<MetalObject>();
            
            if(pos == selectedMetalObjective){
                mo.selected = true;
            }else{
                mo.selected = false;
            }
            pos++;
        }
        int mentalPos = 0;
        foreach(GameObject gm in nearEmotionObjects){
            EnemyBase mo = gm.GetComponent<EnemyBase>();
            
            if(mentalPos == selectedEmotionObjective){
                mo.selected = true;
            }else{
                mo.selected = false;
            }
            mentalPos++;
        }
    }

    void InUpdate(){
        // Debug.Log($"nearEmotionObjects.Count: {nearEmotionObjects.Count}");
    }

    public void OnPushingEmotion(InputAction.CallbackContext context){
        AloMetal amBra = pd.GetAloMetalIfEquipped((int)Metal.BRASS);
        AloMetal amDur = pd.GetAloMetalIfEquipped((int)Metal.DURALUMIN);
        if(pressingQ && context.started && amBra != null && amBra.IsBurning()){
            Debug.Log("Pulsando: PUSHING EMOTION");
            EnemyBase target = nearEmotionObjects[selectedEmotionObjective].GetComponent<EnemyBase>();
            if(target == null) return;
            if(!target.canMentalInterfiere) return;
            target.dampened = true;
            if(amDur != null && amDur.IsBurning()){
                target.duralumined = true;
            }
        }
    }
    public void OnPullingEmotion(InputAction.CallbackContext context){
        AloMetal amZin = pd.GetAloMetalIfEquipped((int)Metal.ZINC);
        AloMetal amDur = pd.GetAloMetalIfEquipped((int)Metal.DURALUMIN);
        if(pressingQ && context.started && amZin != null && amZin.IsBurning()){
            EnemyBase target = nearEmotionObjects[selectedEmotionObjective].GetComponent<EnemyBase>();
            if(target == null) return;
            if(!target.canMentalInterfiere) return;
            target.inflamed = true;
            if(amDur != null && amDur.IsBurning()){
                target.duralumined = true;
            }
        }
    }

    Coroutine pushRoutine = null;
    Coroutine pullRoutine = null;

    public void OnPushingMetal(InputAction.CallbackContext context){
        AloMetal amSte = pd.GetAloMetalIfEquipped((int)Metal.STEEL);
        if (!pressingQ || nearMetalObjects.Count == 0 || amSte == null || !amSte.IsBurning()) return;

        if (context.started && nearMetalObjects.Count > 0){
            pushRoutine = StartCoroutine(ApplyForceContinuously(-1));
        }
        else if (context.canceled && pushRoutine != null){
            StopCoroutine(pushRoutine);
            pushRoutine = null;
        }
    }

    public void OnPullingMetal(InputAction.CallbackContext context){
        AloMetal amIro = pd.GetAloMetalIfEquipped((int)Metal.IRON);
        if (!pressingQ || nearMetalObjects.Count == 0 || amIro == null || !amIro.IsBurning()) return;

        if (context.started && nearMetalObjects.Count > 0){
            pullRoutine = StartCoroutine(ApplyForceContinuously(1));
        }
        else if (context.canceled && pullRoutine != null){
            StopCoroutine(pullRoutine);
            pullRoutine = null;
        }
    }

    IEnumerator ApplyForceContinuously(int directionMultiplier){
        while (pressingQ){
            if (nearMetalObjects.Count == 0) yield break;

            if (selectedMetalObjective < 0 || selectedMetalObjective >= nearMetalObjects.Count){
                selectedMetalObjective = Mathf.Clamp(selectedMetalObjective, 0, nearMetalObjects.Count - 1);
            }

            GameObject target = nearMetalObjects[selectedMetalObjective];
            if (target == null) yield break;

            Vector2 direction = ((Vector2)target.transform.position - rb.position).normalized;

            float thrust = directionMultiplier * 1f;
            AloMetal amDur = pd.GetAloMetalIfEquipped((int)Metal.DURALUMIN);
            if (amDur != null && amDur.IsBurning()){
                thrust *= 3f; 
            }

            Vector2 force = direction * thrust;

            rb.AddForce(force, ForceMode2D.Impulse);

            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
            if (targetRb != null && !targetRb.isKinematic){
                targetRb.AddForce(-force, ForceMode2D.Impulse);
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public void OnParry(InputAction.CallbackContext context){
        if(context.started){
            locuraManager.ConsumeLaudano();
            parryManager.TryParry();
        }
    }

    public Rigidbody2D GetRB() => rb;



    public Image fadingPanel;
    public Transform camera;
    public bool tping = false;

    public void StartTP(Transform target, Transform cameraTarget) {
        StartCoroutine(HandleFadeAndTP(target, cameraTarget));
    }

    private IEnumerator HandleFadeAndTP(Transform target, Transform cameraTarget) {
        tping = true;
        yield return StartCoroutine(FadeOut());

        // Teletransportar jugador y cámara
        transform.position = target.position;

        Vector3 camTarget = cameraTarget.position;
        camTarget.z = camera.position.z;
        camera.position = camTarget;

        yield return new WaitForSecondsRealtime(0.5f);
        yield return StartCoroutine(FadeIn());
        tping = false;
    }

    private IEnumerator FadeOut() {
        Color c = fadingPanel.color;
        while (c.a < 1f) {
            c.a += 0.2f;
            c.a = Mathf.Clamp01(c.a);
            fadingPanel.color = c;
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    private IEnumerator FadeIn() {
        Color c = fadingPanel.color;
        while (c.a > 0f) {
            c.a -= 0.2f;
            c.a = Mathf.Clamp01(c.a);
            fadingPanel.color = c;
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }
}
