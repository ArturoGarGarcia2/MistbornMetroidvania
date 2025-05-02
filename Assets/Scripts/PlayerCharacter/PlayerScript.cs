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

public class PlayerScript : MonoBehaviour{
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
    [SerializeField] int lifes = 50;
    public int actual_lifes = 1;
    float targetTime;
    bool dead = false;
    [SerializeField] [Range(5,25)] float hurt_thrust;
    float short_term_thrust;

    Rigidbody2D rb;
    Animator playerAnimator;
    SpriteRenderer sr;

    
    bool doubleJumped;

    public bool enterFuente;
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
        "Duraluminio",
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

        // Duraluminio
        "Hace perder al brumoso sus propias reservas alománticas de los metales que esté quemando en un estallido de poder.\nEste metal puede ser muy poderoso, pero peligroso dependiendo del metal que se queme.",

        // Atium
        "Permite al usuario ver unos segundos en el futuro, pudiendo prever qué pasará.\nUn metal muy poderoso, pero se consume con mucha facilidad."
    };
    public TextMeshProUGUI txtMetalRueda;
    float curacionOroTiempo = 20;
    float curacionOroTiempoTranscurrido = 20;

    public Dictionary<int, int> slotMetalAlo = new Dictionary<int, int>();
    public Dictionary<string,int> cantidadesAlo = new Dictionary<string, int>();
    public Dictionary<string,int> capacidadesAlo = new Dictionary<string, int>();
    public Dictionary<int, int> estadoSlotAlo = new Dictionary<int, int>();
    public Dictionary<string, int> velocidadQuemado = new Dictionary<string, int>();
    public Sprite[] spritesBarrasMetales;
    public GameObject barraRueda;

    public Dictionary<int,int> slotMetalFeru = new Dictionary<int, int>();
    public Dictionary<string,int> cantidadesFeru = new Dictionary<string, int>();
    public Dictionary<string,int> capacidadesFeru = new Dictionary<string, int>();
    public Dictionary<int,int> estadoSlotFeru = new Dictionary<int, int>();
    public Dictionary<string, int> velocidadGuardado = new Dictionary<string, int>();
    public Dictionary<string, int> velocidadDecantado = new Dictionary<string, int>();

    public Dictionary<int, int> slotMetalHema = new Dictionary<int, int>();
    public Dictionary<int, int> vialesVida = new Dictionary<int, int>();
    
    public List<Objeto> objetosDesbloqueados;

    PauseManager pauseManager;

    public bool nearAnyNPC = false;
    public GameObject nearNPC = null;
    public bool inNPC = false;
    public bool nextFrase = false;

    public string faseActual;
    public PlayerData pd;


    void Start(){
        pd = new PlayerData(1);
        // SetMetales();
        // SetDictionaries();

        cvMetalWheel.SetActive(false);
        PlayerPrefs.SetInt("Coins", 0);
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        
        lifes = DatabaseManager.Instance.GetInt("SELECT max_health FROM file WHERE id = 1;");
        actual_lifes = lifes;

        //LoadFromDatabase();

        transform.position = GetPuntoControlDesdeDB();
        pauseManager = FindObjectOfType<PauseManager>();
        StartCoroutine(ActualizarCantidadesCadaMedioSegundo());

        faseActual = DatabaseManager.Instance.GetString("SELECT phase FROM file WHERE id = 1;");
    }

    void SetDictionaries(){
        // ALOMANCIA, FERUQUIMIA Y HEMALURGIA
        for (int i = 1; i <= 8; i++){
            int metalId = DatabaseManager.Instance.GetInt($"SELECT metal_id FROM metal_file WHERE file_id = 1 AND slot_a = {i};");
            slotMetalAlo.Add(i,metalId==-1?0:metalId);
        }
        for (int i = 1; i <= 4; i++){
            int metalId = DatabaseManager.Instance.GetInt($"SELECT metal_id FROM metal_file WHERE file_id = 1 AND slot_f = {i};");
            slotMetalFeru.Add(i,metalId==-1?0:metalId);
        }
        for (int i = 1; i <= 7; i++){
            int metalId = DatabaseManager.Instance.GetInt($"SELECT metal_id FROM metal_file WHERE file_id = 1 AND slot_h = {i};");
            slotMetalHema.Add(i,metalId==-1?0:metalId);
        }

        // VIALES
        int numViales = DatabaseManager.Instance.GetInt($"SELECT vials FROM file WHERE id = 1;");
        for(int i = 1; i <= 6; i++){
            vialesVida.Add(i, i <=numViales ? 1 : -1);
        }

        // OBJETOS
        List<int> idObjetosDesbloqueados = DatabaseManager.Instance.GetIntListFromQuery("SELECT object_id FROM unlocked_object WHERE file_id = 1;");
        foreach(int i in idObjetosDesbloqueados){
            Dictionary<string,object> objetoRaw = DatabaseManager.Instance.GetSingleRowFromQuery($"SELECT * FROM object WHERE id = {i}");

            objetosDesbloqueados.Add(new Objeto(Convert.ToInt32(objetoRaw["id"]), objetoRaw["name"].ToString(), objetoRaw["description"].ToString()));
        }
    }

    public Vector3 GetPuntoControlDesdeDB(){
        Vector3 posicion = Vector3.zero;

        int lastCheckpoint = DatabaseManager.Instance.GetInt($"SELECT checkpoint_id FROM file WHERE id = 1;");

        Debug.Log($"EL ID DEL CHECKPOINT: {lastCheckpoint}");

        string queryX = $"SELECT x_pos FROM checkpoint WHERE id = {lastCheckpoint}";
        object resultadoX = DatabaseManager.Instance.ExecuteScalar(queryX);
        string queryY = $"SELECT y_pos FROM checkpoint WHERE id = {lastCheckpoint}";
        object resultadoY = DatabaseManager.Instance.ExecuteScalar(queryY);

        if (resultadoX == null || resultadoY == null){
            Debug.LogError("No se pudo recuperar la posición del punto de control desde la base de datos.");
        }else{
            float posX = Convert.ToSingle(resultadoX);
            float posY = Convert.ToSingle(resultadoY);

            posicion = new Vector3(posX, posY, 0);
        }

        return posicion;
    }

    void SetMetales(){
        cantidadesAlo.Clear();
        capacidadesAlo.Clear();
        estadoSlotAlo.Clear();
        velocidadQuemado.Clear();
        
        cantidadesFeru.Clear();
        capacidadesFeru.Clear();
        velocidadGuardado.Clear();
        velocidadDecantado.Clear();
        estadoSlotFeru.Clear();

        for(int i = 0 ; i < metales.Length ; i++){
            int cantidadA = DatabaseManager.Instance.GetInt($"SELECT mf.amount_a "+
                                                            $"FROM metal_file mf "+
                                                            $"JOIN metals m ON mf.metal_id = m.id "+
                                                            $"WHERE mf.file_id = 1 AND m.name = '{metales[i]}';");
            cantidadesAlo.Add(metales[i], cantidadA);
            int capacidadA = DatabaseManager.Instance.GetInt($"SELECT mf.capacity_a "+
                                                            $"FROM metal_file mf "+
                                                            $"JOIN metals m ON mf.metal_id = m.id "+
                                                            $"WHERE mf.file_id = 1 AND m.name = '{metales[i]}';");
            capacidadesAlo.Add(metales[i], capacidadA);

            int cantidadF = DatabaseManager.Instance.GetInt($"SELECT mf.amount_f "+
                                                            $"FROM metal_file mf "+
                                                            $"JOIN metals m ON mf.metal_id = m.id "+
                                                            $"WHERE mf.file_id = 1 AND m.name = '{metales[i]}';");
            cantidadesFeru.Add(metales[i], cantidadF);
            int capacidadF = DatabaseManager.Instance.GetInt($"SELECT mf.capacity_f "+
                                                            $"FROM metal_file mf "+
                                                            $"JOIN metals m ON mf.metal_id = m.id "+
                                                            $"WHERE mf.file_id = 1 AND m.name = '{metales[i]}';");
            capacidadesFeru.Add(metales[i], capacidadF);

            // Asignación de velocidades de guardado y decantado, estas pueden ser definidas según tu lógica de juego
            velocidadQuemado.Add(metales[i], ObtenerVelocidadQuemado(metales[i]));

            velocidadGuardado.Add(metales[i], ObtenerVelocidadGuardado(metales[i]));
            velocidadDecantado.Add(metales[i], ObtenerVelocidadDecantado(metales[i]));
        }
        for(int i = 1; i <= 8 ; i++){
            estadoSlotAlo.Add(i,0);
        }
        for(int i = 1; i <= 4 ; i++){
            estadoSlotFeru.Add(i,0);
        }
    }

    int ObtenerVelocidadQuemado(string metal) {
        switch (metal) {
            case "Oro": return 1;
            case "Aluminio": return 2;
            case "Atium": return 1;
            
            default: return 3;
        }
    }

    int ObtenerVelocidadGuardado(string metal) {
        switch (metal) {
            case "Hierro": return 3;
            case "Acero": return 3;
            case "Estaño": return 2;
            case "Peltre": return 1;
            case "Zinc": return 3;
            case "Latón": return 3;
            case "Cobre": return 2;
            case "Bronce": return 2;
            case "Cadmio": return 1;
            case "Bendaleo": return 4;
            case "Oro": return 1;
            case "Electro": return 1;
            case "Cromo": return 2;
            case "Nicrosil": return 2;
            case "Aluminio": return 2;
            case "Duraluminio": return 2;
            case "Atium": return 1;
            default: return 0;
        }
    }

    int ObtenerVelocidadDecantado(string metal) {
        switch (metal) {
            case "Hierro": return 2;
            case "Acero": return 3;
            case "Estaño": return 1;
            case "Peltre": return 5;
            case "Zinc": return 2;
            case "Latón": return 2;
            case "Cobre": return 1;
            case "Bronce": return 1;
            case "Cadmio": return 1;
            case "Bendaleo": return 4;
            case "Oro": return 6;
            case "Electro": return 3;
            case "Cromo": return 4;
            case "Nicrosil": return 4;
            case "Aluminio": return 4;
            case "Duraluminio": return 6;
            case "Atium": return 6;
            default: return 1;
        }
    }

    public void OnFeruUp(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused){
            pd.ChangeFeruMetalStatus(1);
        }
    }
    public void OnFeruRight(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused){
            pd.ChangeFeruMetalStatus(2);
        }
    }
    public void OnFeruDown(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused){
            pd.ChangeFeruMetalStatus(3);
        }
    }
    public void OnFeruLeft(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused){
            pd.ChangeFeruMetalStatus(4);
        }
    }

    IEnumerator ActualizarCantidadesCadaMedioSegundo() {
        while (true) {
            for (int i = 1; i <= pd.GetFeruSlots().Length; i++) {        
                pd.GetFeruMetalInSlot(i)?.UseMetalmind();
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void FixedUpdate(){
        if(short_term_thrust != 0){short_term_thrust *= 0.8f;}
        int map_active = PlayerPrefs.GetInt("map_active");

        if(map_active==0){
            if (isDashing){
                Dash();
            }else{
                playerAnimator.SetBool("Grounded", grounded);
                playerAnimator.SetFloat("Velocity", rb.velocity.y);

                if(!dead){
                    Fuente();
                    Move();
                    Jump();
                    Attack1();
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

        curacionOroTiempo = HasClavo("Electro") ? 15 : 20;
        if (HasClavo("Oro")) {
            CuracionOro();
        }
    }

    void Update(){
        SetMetalWheel();
    }

    void CuracionOro(){
        curacionOroTiempoTranscurrido -= Time.deltaTime;
        
        if (curacionOroTiempoTranscurrido <= 0f) {
            
            if (actual_lifes < lifes) {
                actual_lifes++;
            }

            curacionOroTiempoTranscurrido = curacionOroTiempo;
        }
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
                    if(!am.IsBurning()){
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
                    if(!am.IsBurning()){
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
        }
    }

    void SelectAloMetal(){
        pd.ChangeAloMetalBurning(selectedAloMetalSlot);
    }

    public void OnMove(InputAction.CallbackContext context){
        if(selectingMetal){
            movementInput.x = 0;
            movementInput.y = 0;
            return;
        }
        movementInput = context.ReadValue<Vector2>();
    }
    void Move(){
        float multiplierAcero = !HasClavo("Acero") ? 1f : (HasClavo("Electro") ? 1.5f : 1.2f);
        rb.velocity = new Vector2(short_term_thrust*2 + movementInput.x * speed * multiplierAcero, rb.velocity.y);

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
        if(context.started && selectingMetal){
            Debug.Log("SELECCIONANDO EL METAL ALOMÁNTICO");
            SelectAloMetal();
            return;
        }

        if (context.started && !pauseManager.isPaused) {
            if (grounded){
                jumped = true;
            } else if (coins > 0){  // Solo permite saltar si hay monedas disponibles
                doubleJumped = true;
                coins--;  // Resta una moneda de la variable local
                PlayerPrefs.SetInt("Coins", coins);  // Guarda el nuevo valor en PlayerPrefs
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
        if (context.performed && canDash){
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
            enterFuente = context.action.triggered;
    }

    void Fuente() {
        if (nearAnyNPC && enterFuente){
            inNPC = true;
            var npcBehaviour = nearNPC.GetComponent<NPCBehaviour>();
            if (npcBehaviour != null){
                npcBehaviour.StartDialogue();
            }
            Debug.Log("Interactuando o ya vo sabe");
            speed = 0;
        }
        if (enterFuente && inFuente) {
            // pd.SaveToDatabase();
            pd.EnterFuente();
            speed = 0;

            DatabaseManager.Instance.ExecuteNonQuery(
                $"UPDATE file SET max_health = {lifes} WHERE id = 1;"
            );
            actual_lifes = lifes;
            
            canExitFuente = false;
            enterFuente = false;
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

    public void SaveToDatabase(){
        foreach (var slot in new List<int>(estadoSlotFeru.Keys)) {
            estadoSlotFeru[slot] = 0;
        }

        foreach (var kvp in cantidadesFeru){
            string nombreMetal = kvp.Key;
            int cantidad = kvp.Value;

            string query = $"UPDATE metal_file SET amount_f = {cantidad} WHERE file_id = 1 AND name = '{nombreMetal}';";
            DatabaseManager.Instance.ExecuteNonQuery(query);
        }

        DatabaseManager.Instance.ExecuteNonQuery(
            "UPDATE metal_file SET slot_a = 0 WHERE file_id = 1;"
        );
        foreach (var kvp in slotMetalAlo){
            int slot = kvp.Key;
            int metalId = kvp.Value;

            string query = "";
            // Debug.Log($"metal: {nombreMetal} - cantidad: {cantidad}");
            if(metalId != 0){
                query = $"UPDATE metal_file SET slot_a = {slot} WHERE file_id = 1 AND metal_id = '{metalId}';";
                DatabaseManager.Instance.ExecuteNonQuery(query);
            }
        }

        DatabaseManager.Instance.ExecuteNonQuery(
            "UPDATE metal_file SET slot_f = 0 WHERE file_id = 1;"
        );
        foreach (var kvp in slotMetalFeru){
            int slot = kvp.Key;
            int metalId = kvp.Value;

            string query = "";
            // Debug.Log($"metal: {nombreMetal} - cantidad: {cantidad}");
            if(metalId != 0){
                query = $"UPDATE metal_file SET slot_f = {slot} WHERE file_id = 1 AND metal_id = '{metalId}';";
                DatabaseManager.Instance.ExecuteNonQuery(query);
            }
        }

        DatabaseManager.Instance.ExecuteNonQuery(
            "UPDATE metal_file SET slot_h = 0 WHERE file_id = 1;"
        );
        foreach (var kvp in slotMetalHema){
            int slotHema = kvp.Key;
            int metalId = kvp.Value;

            string query = "";
            // Debug.Log($"metal: {nombreMetal} - cantidad: {cantidad}");
            if(metalId != 0){
                query = $"UPDATE metal_file SET slot_h = {slotHema} WHERE file_id = 1 AND metal_id = '{metalId}';";
                DatabaseManager.Instance.ExecuteNonQuery(query);
            }
        }

        int vialesDesbloqueados = 0;
        foreach (var kvp in vialesVida){
            int numVial = kvp.Key;
            int estadoV = kvp.Value;

            if(estadoV != -1){
                vialesDesbloqueados++;
            }
        }
        DatabaseManager.Instance.ExecuteNonQuery(
            $"UPDATE file SET vials = {vialesDesbloqueados} WHERE id = 1;"
        );
        
        Debug.Log("Datos guardados en la base de datos.");
        SetMetales();
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

    void Hurt(int damage) {
        if (!isInvulnerable) {

            int dado = UnityEngine.Random.Range(1,101);
            Debug.Log($"Dado: {dado}");
            Debug.Log($"Atium: {HasClavo("Atium")}");
            if(!HasClavo("Atium") || dado >= (HasClavo("Electro") ? 15 : 10)){
                RestarVida(damage);
            }else{
                Debug.Log("FEELING LUCKY");
            }
            

            if (actual_lifes <= 0){
                dead = true;
                sr.color = Color.white;
                playerAnimator.SetTrigger("DeadTrigger");
            }else{
                StartCoroutine(Invulnerability());
            }
        }
    }

    void RestarVida(int damage){
        actual_lifes -= damage;
        short_term_thrust = HasClavo("Hierro") ? 0f : facingRight ? -hurt_thrust : hurt_thrust;
        sr.color = Color.red;
    }

    // Corrutina para gestionar el tiempo de invulnerabilidad
    IEnumerator Invulnerability() {
        isInvulnerable = true;
        sr.color = new Color(1, 0, 0, 0.5f);
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
        sr.color = Color.white;
    }

    void OnTriggerStay2D(Collider2D other){
        if (other.tag == "Floor"){
            grounded = true;
            canDash = true;
        }
    }


    void OnTriggerEnter2D(Collider2D other){
        // if (other.tag == "Enemy_Attack" && !dead){
        //     Debug.Log("other: " + other.transform.parent);

        //     IAttacker attacker = other.GetComponentInParent<IAttacker>();

        //     if (attacker != null){
        //         int damage = attacker.GetDamage();
        //         Debug.Log("Damage taken: " + damage);
        //         Hurt(damage);
        //     }else{
        //         Debug.LogWarning("El objeto atacante no implementa IAttacker.");
        //     }
        // }

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
            Debug.Log($"In NPC");
            if (nearNPC != null) {
                var npcBehaviour = nearNPC.GetComponent<NPCBehaviour>();
                if (npcBehaviour != null){
                    Debug.Log("NPCBehaviour encontrado: " + npcBehaviour.name);
                } else {
                    Debug.LogWarning("No se encontró NPCBehaviour en el GameObject cerca.");
                }
            }
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
    }

    public void OnNextFrase(InputAction.CallbackContext context){
        if(nearAnyNPC && inNPC && context.action.triggered){
            nextFrase = true;
        }else{
            nextFrase = false;
        }
    }

    bool HasClavo(string metal){
        int metalId = GetIdMetal(metal);
        int slotOcupado = -1;
        foreach (var kv in slotMetalHema) {
            if (kv.Value == metalId) {
                slotOcupado = kv.Key;
                break;
            }
        }
        return slotOcupado != 0;
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

    public int GetNumClavos(){
        int acu = 0;
        foreach (var kv in slotMetalHema){
            if (kv.Value > 0){
                acu++;
            }
        }
        return acu;
    }

    public bool QuemandoMetal(string nombreMetal){
        int idMetal = GetIdMetal(nombreMetal);
        foreach(var kv in slotMetalAlo){
            if (kv.Value == idMetal){
                int slot = kv.Key;
                if(estadoSlotAlo[slot] == 1 && cantidadesAlo[nombreMetal]>0){
                    return true;
                }
            }
        }
        return false;
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

        // int count = DatabaseManager.Instance.GetInt($@"
        //     SELECT COUNT(*) FROM achieved_event 
        //     WHERE file_id = {pd.GetFileId()} 
        //     AND event_id = (SELECT id FROM event WHERE name = '{nombreEvento}');
        // ");

        // if (count == 0){
        //     DatabaseManager.Instance.ExecuteNonQuery($@"
        //         INSERT INTO achieved_event (file_id, event_id)
        //         VALUES (
        //             {pd.GetFileId()},
        //             (SELECT id FROM event WHERE name = '{nombreEvento}')
        //         );
        //     ");
        //     Debug.Log($"Evento '{nombreEvento}' registrado para file_id {pd.GetFileId()}");
        // } else {
        //     Debug.Log($"Evento '{nombreEvento}' ya estaba registrado.");
        // }
    }
}
