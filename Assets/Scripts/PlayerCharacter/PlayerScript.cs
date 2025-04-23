using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
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
    Enemy_Hurt EH;
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

    float curacionOroTiempo = 20;
    float curacionOroTiempoTranscurrido = 20;

    public Dictionary<string,int> cantidadesFeru = new Dictionary<string, int>();
    public Dictionary<string,int> capacidadesFeru = new Dictionary<string, int>();
    public Dictionary<int,int> estadoSlotFeru = new Dictionary<int, int>();
    public Dictionary<string, int> velocidadGuardado = new Dictionary<string, int>();
    public Dictionary<string, int> velocidadDecantado = new Dictionary<string, int>();

    public Dictionary<int, int> slotMetalAlo = new Dictionary<int, int>();
    public Dictionary<int,int> slotMetalFeru = new Dictionary<int, int>();
    public Dictionary<int, int> slotMetalHema = new Dictionary<int, int>();
    public Dictionary<int, int> vialesVida = new Dictionary<int, int>();
    
    public List<Objeto> objetosDesbloqueados;

    PauseManager pauseManager;

    public bool nearAnyNPC = false;
    public GameObject nearNPC = null;
    public bool inNPC = false;
    public bool nextFrase = false;


    void Start(){
        SetMetales();
        SetDictionaries();

        List<int> idObjetosDesbloqueados = DatabaseManager.Instance.GetIntListFromQuery("SELECT objeto_id FROM objeto_desbloqueado WHERE archivo_id = 1;");
        foreach(int i in idObjetosDesbloqueados){
            Dictionary<string,object> objetoRaw = DatabaseManager.Instance.GetSingleRowFromQuery($"SELECT * FROM objeto WHERE id = {i}");

            Debug.Log($"result: {objetoRaw["nombre"]} - {objetoRaw["descripcion"]}");
        }

        cvMetalWheel.SetActive(false);
        PlayerPrefs.SetInt("Coins", 0);
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        
        lifes = DatabaseManager.Instance.GetInt("SELECT salud_maxima FROM archivo WHERE id = 1;");
        actual_lifes = lifes;

        LoadFromDatabase();

        transform.position = CargarPuntoControlDesdeDB();
        pauseManager = FindObjectOfType<PauseManager>();
        StartCoroutine(ActualizarCantidadFeruCadaMedioSegundo());
    }

    void SetDictionaries(){
        // ALOMANCIA, FERUQUIMIA Y HEMALURGIA
        for (int i = 1; i <= 8; i++){
            int metalId = DatabaseManager.Instance.GetInt($"SELECT metal_id FROM metal_archivo WHERE archivo_id = 1 AND slot_a = {i};");
            slotMetalAlo.Add(i,metalId==-1?0:metalId);
        }
        for (int i = 1; i <= 4; i++){
            int metalId = DatabaseManager.Instance.GetInt($"SELECT metal_id FROM metal_archivo WHERE archivo_id = 1 AND slot_f = {i};");
            slotMetalFeru.Add(i,metalId==-1?0:metalId);
        }
        for (int i = 1; i <= 7; i++){
            int metalId = DatabaseManager.Instance.GetInt($"SELECT metal_id FROM metal_archivo WHERE archivo_id = 1 AND slot_h = {i};");
            slotMetalHema.Add(i,metalId==-1?0:metalId);
        }

        // VIALES
        int numViales = DatabaseManager.Instance.GetInt($"SELECT viales FROM archivo WHERE id = 1;");
        for(int i = 1; i <= 6; i++){
            vialesVida.Add(i, i <=numViales ? 1 : -1);
        }
    }

    public Vector3 CargarPuntoControlDesdeDB(){
        Vector3 posicion = Vector3.zero;

        int lastCheckpoint = DatabaseManager.Instance.GetInt($"SELECT punto_control_id FROM archivo WHERE id = 1;");

        string queryX = "SELECT posicion_x FROM punto_control WHERE id = @param0";
        object resultadoX = DatabaseManager.Instance.ExecuteScalar(queryX, lastCheckpoint);
        string queryY = "SELECT posicion_y FROM punto_control WHERE id = @param0";
        object resultadoY = DatabaseManager.Instance.ExecuteScalar(queryY, lastCheckpoint);

        if (resultadoX == null || resultadoY == null){
            Debug.LogError("No se pudo recuperar la posición del punto de control desde la base de datos.");
        }else{
            float posX = Convert.ToSingle(resultadoX);
            float posY = Convert.ToSingle(resultadoY);

            posicion = new Vector3(posX, posY, 0);
        }

        return posicion;
    }


    public void LoadFromDatabase(){
        object saludMaxima = DatabaseManager.Instance.ExecuteScalar("SELECT salud_maxima FROM archivo WHERE id = 1;");
        object puntoControl = DatabaseManager.Instance.ExecuteScalar("SELECT punto_control_id FROM archivo WHERE id = 1;");

        if (saludMaxima != null && saludMaxima != DBNull.Value)
            PlayerPrefs.SetInt("Health", Convert.ToInt32(saludMaxima));
        else
            PlayerPrefs.SetInt("Health", 50); // Valor por defecto si no hay datos en la BD

        if (puntoControl != null && puntoControl != DBNull.Value)
            PlayerPrefs.SetInt("LastCheckpoint", Convert.ToInt32(puntoControl));
        else
            PlayerPrefs.SetInt("LastCheckpoint", 0);
    }

    void SetMetales(){
        cantidadesFeru.Clear();
        capacidadesFeru.Clear();
        velocidadGuardado.Clear();
        velocidadDecantado.Clear();
        estadoSlotFeru.Clear();

        for(int i = 0 ; i < metales.Length ; i++){
            int cantidadF = DatabaseManager.Instance.GetInt($"SELECT cantidad_f FROM metal_archivo WHERE archivo_id = 1 AND nombre = '{metales[i]}';");
            cantidadesFeru.Add(metales[i], cantidadF);
            int capacidadF = DatabaseManager.Instance.GetInt($"SELECT capacidad_f FROM metal_archivo WHERE archivo_id = 1 AND nombre = '{metales[i]}';");
            capacidadesFeru.Add(metales[i], capacidadF);

            // Asignación de velocidades de guardado y decantado, estas pueden ser definidas según tu lógica de juego
            velocidadGuardado.Add(metales[i], ObtenerVelocidadGuardado(metales[i]));
            velocidadDecantado.Add(metales[i], ObtenerVelocidadDecantado(metales[i]));
        }
        for(int i = 1; i <= 4 ; i++){
            estadoSlotFeru.Add(i,0);
        }
    }

    int ObtenerVelocidadGuardado(string metal) {
        switch (metal) {
            case "Oro": return 1;
            case "Aluminio": return 2;
            case "Atium": return 1;
            // Agregar más metales y sus velocidades
            default: return 3; // Valor por defecto
        }
    }

    int ObtenerVelocidadDecantado(string metal) {
        switch (metal) {
            case "Oro": return 6;
            case "Aluminio": return 4;
            case "Atium": return 6;
            // Agregar más metales y sus velocidades
            default: return 1; // Valor por defecto
        }
    }

    public void OnFeruUp(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused){
            ChangeEstadoSlotFeru(1);
        }
    }
    public void OnFeruRight(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused){
            ChangeEstadoSlotFeru(2);
        }
    }
    public void OnFeruDown(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused){
            ChangeEstadoSlotFeru(3);
        }
    }
    public void OnFeruLeft(InputAction.CallbackContext context){
        if (context.started && !pauseManager.isPaused){
            ChangeEstadoSlotFeru(4);
        }
    }

    void ChangeEstadoSlotFeru(int slot){
        int metalId = slotMetalFeru[slot];
        string nombreMetal = GetNombreMetal(metalId);

        if(metalId != null){
            estadoSlotFeru[slot] += 1;
            if(estadoSlotFeru[slot] > 1){
                estadoSlotFeru[slot] = -1;
            }
            if(estadoSlotFeru[slot] == 1 && cantidadesFeru[nombreMetal] == capacidadesFeru[nombreMetal]){
                estadoSlotFeru[slot] = 0;
            }
            if(estadoSlotFeru[slot] == -1 && cantidadesFeru[nombreMetal] == 0){
                estadoSlotFeru[slot] = 0;
            }
        }
    }

    IEnumerator ActualizarCantidadFeruCadaMedioSegundo() {
        while (true) {
            for (int i = 1; i <= estadoSlotFeru.Count; i++) {
                ModifyCantidadFeru(i);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void ModifyCantidadFeru(int slot) {
        int metalId = slotMetalFeru[slot];
        string nombreMetal = GetNombreMetal(metalId);

        if(nombreMetal != null){
            if (estadoSlotFeru[slot] == 1 && cantidadesFeru[nombreMetal] < capacidadesFeru[nombreMetal]){
                int velocidad = velocidadGuardado.ContainsKey(nombreMetal) ? velocidadGuardado[nombreMetal] : 1;
                cantidadesFeru[nombreMetal] += estadoSlotFeru[slot] * velocidad;
                if(cantidadesFeru[nombreMetal] > capacidadesFeru[nombreMetal]){
                    cantidadesFeru[nombreMetal] = capacidadesFeru[nombreMetal];
                    estadoSlotFeru[slot] = 0;
                }
            }else if (estadoSlotFeru[slot] == -1 && cantidadesFeru[nombreMetal] > 0){
                int velocidad = velocidadDecantado.ContainsKey(nombreMetal) ? velocidadDecantado[nombreMetal] : 1;
                cantidadesFeru[nombreMetal] += estadoSlotFeru[slot] * velocidad;
                if(cantidadesFeru[nombreMetal] <= 0){
                    cantidadesFeru[nombreMetal] = 0;
                    estadoSlotFeru[slot] = 0;
                }
            }
        }else{
            estadoSlotFeru[slot] = 0;
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
                    //Hurt();
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
        SetMetalWheel();

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
        for(int i = 1; i <= slotsMetalWheel.Length; i++){
            // int idMetal = DatabaseManager.Instance.GetInt($"SELECT metal_id FROM metal_archivo WHERE archivo_id = 1 AND slot_a = {i};");
            int idMetal = slotMetalAlo[i];
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
                    slotsMetalWheel[i-1].color = Color.red;
                    slotsMetalWheelMenu[i-1].color = Color.red;
                }else{
                    slotsMetalWheel[i-1].color = Color.white;
                    slotsMetalWheelMenu[i-1].color = Color.white;
                }
            }
        }
    }

    public void OnOpenMetalWheel(InputAction.CallbackContext context){
        if (context.started){
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
            Debug.Log($"selectedAloMetalSlot: {selectedAloMetalSlot}");
        }
    }

    public void OnMove(InputAction.CallbackContext context){
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
        int vialDisponible = 0;
        foreach (var kvp in vialesVida){
            int numVial = kvp.Key;
            int estadoV = kvp.Value;

            if(estadoV != -1 && estadoV != 0){
                vialDisponible = numVial;
            }
        }
        if(vialDisponible != 0){
            vialesVida[vialDisponible] = 0;
            int potencia = DatabaseManager.Instance.GetInt("SELECT potencia_vial FROM archivo WHERE id = 1;");
            actual_lifes+=potencia;
            if(actual_lifes > lifes){
                actual_lifes = lifes;
            }
        }
    }

    public void OnEnterFuente(InputAction.CallbackContext context){
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
            SaveToDatabase();
            List<int> vialesParaRestaurar = new List<int>();
            foreach (var kvp in vialesVida){
                if (kvp.Value != -1){
                    vialesParaRestaurar.Add(kvp.Key);
                }
            }

            foreach (int numVial in vialesParaRestaurar){
                vialesVida[numVial] = 1;
            }
            speed = 0;

            DatabaseManager.Instance.ExecuteNonQuery(
                $"UPDATE archivo SET salud_maxima = {lifes} WHERE id = 1;"
            );
            actual_lifes = lifes;
            
            canExitFuente = false;
            enterFuente = false;
            StartCoroutine(EnableExitFuente(1f));
            int visited = (int)(long)DatabaseManager.Instance.ExecuteScalar("SELECT COUNT(*) FROM punto_control_visitado WHERE archivo_id = 1 AND punto_control_id = "+fuenteActual+";");
            if(visited == 0){
                DatabaseManager.Instance.ExecuteNonQuery("INSERT INTO punto_control_visitado (archivo_id, punto_control_id) VALUES (1, "+fuenteActual+");");
            }
            DatabaseManager.Instance.ExecuteNonQuery($"UPDATE archivo SET punto_control_id = {fuenteActual} WHERE id = 1;");
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

            string query = $"UPDATE metal_archivo SET cantidad_f = {cantidad} WHERE archivo_id = 1 AND nombre = '{nombreMetal}';";
            DatabaseManager.Instance.ExecuteNonQuery(query);
        }

        DatabaseManager.Instance.ExecuteNonQuery(
            "UPDATE metal_archivo SET slot_a = 0 WHERE archivo_id = 1;"
        );
        foreach (var kvp in slotMetalAlo){
            int slot = kvp.Key;
            int metalId = kvp.Value;

            string query = "";
            // Debug.Log($"metal: {nombreMetal} - cantidad: {cantidad}");
            if(metalId != 0){
                query = $"UPDATE metal_archivo SET slot_a = {slot} WHERE archivo_id = 1 AND metal_id = '{metalId}';";
                DatabaseManager.Instance.ExecuteNonQuery(query);
            }
        }

        DatabaseManager.Instance.ExecuteNonQuery(
            "UPDATE metal_archivo SET slot_f = 0 WHERE archivo_id = 1;"
        );
        foreach (var kvp in slotMetalFeru){
            int slot = kvp.Key;
            int metalId = kvp.Value;

            string query = "";
            // Debug.Log($"metal: {nombreMetal} - cantidad: {cantidad}");
            if(metalId != 0){
                query = $"UPDATE metal_archivo SET slot_f = {slot} WHERE archivo_id = 1 AND metal_id = '{metalId}';";
                DatabaseManager.Instance.ExecuteNonQuery(query);
            }
        }

        DatabaseManager.Instance.ExecuteNonQuery(
            "UPDATE metal_archivo SET slot_h = 0 WHERE archivo_id = 1;"
        );
        foreach (var kvp in slotMetalHema){
            int slotHema = kvp.Key;
            int metalId = kvp.Value;

            string query = "";
            // Debug.Log($"metal: {nombreMetal} - cantidad: {cantidad}");
            if(metalId != 0){
                query = $"UPDATE metal_archivo SET slot_h = {slotHema} WHERE archivo_id = 1 AND metal_id = '{metalId}';";
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
            $"UPDATE archivo SET viales = {vialesDesbloqueados} WHERE id = 1;"
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
        if (other.tag == "Enemy_Attack" && !dead){
            Debug.Log("other: " + other.transform.parent);

            IAttacker attacker = other.GetComponentInParent<IAttacker>();

            if (attacker != null){
                int damage = attacker.GetDamage();
                Debug.Log("Damage taken: " + damage);
                Hurt(damage);
            }else{
                Debug.LogWarning("El objeto atacante no implementa IAttacker.");
            }
        }

        if (other.tag == "Fuente" && !dead) {
            inFuente = true;
            // Intentar convertir la segunda parte del nombre a un número entero
            string[] nombrePartes = other.name.Split(" ");
            if (nombrePartes.Length > 1) {
                // Intentar convertir a int
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

    public void OnNPC(InputAction.CallbackContext context){
        // if (nearAnyNPC && context.action.triggered){
        //     inNPC = true;
        //     var npcBehaviour = nearNPC.GetComponent<NPCBehaviour>();
        //     if (npcBehaviour != null){
        //         npcBehaviour.StartDialogue();
        //     }
        //     Debug.Log("Interactuando o ya vo sabe");
        //     speed = 0;
        // }
    }

    public void OnNextFrase(InputAction.CallbackContext context){
        if(nearAnyNPC && inNPC && context.action.triggered){
            nextFrase = true;
        }else{
            nextFrase = false;
        }
    }

    bool HasClavo(string metal){
        int metalId = DatabaseManager.Instance.GetInt($"SELECT metal_id FROM metal_archivo WHERE archivo_id = 1 AND nombre = '{metal}';");
        int slotOcupado = -1;
        foreach (var kv in slotMetalHema) {
            if (kv.Value == metalId) {
                slotOcupado = kv.Key;
                break;
            }
        }
        return slotOcupado != 0;
    }

    string GetNombreMetal(int metalId){
        return DatabaseManager.Instance.GetString(
            $"SELECT nombre FROM metal_archivo WHERE archivo_id = 1 AND metal_id = {metalId};"
        );
    }
}
