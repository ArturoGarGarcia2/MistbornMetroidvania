using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

public class PauseManager : MonoBehaviour{

    private string[] nombreMetalAlo = new string[] {
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
        "Apacigua las emociones del objetivo.\nDependiendo del objetivo, puede o hacer menos daño al brumoso o ignorarlo por completo.",

        // Cobre
        "Oculta la alomancia del brumoso.\nEsto permite ocultar al brumoso de los enemigos que puedan detectar la alomancia.",

        // Bronce
        "Muestra la alomancia de los brumosos cercanos.\nPermite conocer la alomancia que usan los brumosos cercanos.",

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
    private string[] nombreMetalFeru = new string[] {
        "Brazal de Hierro",
        "Bracil de Acero",
        "Esclava de Estaño",
        "Ajorca de Peltre",
        "Pulsera de Zinc",
        "Bracil de Latón",
        "Brazal de Cobre",
        "Pulsera de Bronce",
        "Brazalete de Cadmio",
        "Bracil de Bendaleo",
        "Ajorca de Oro",
        "Brazalete de Electro",
        "Brazal de Cromo",
        "Esclava de Nicrosil",
        "Brazalete de Aluminio",
        "Pulsera de Duraluminio",
        "Esclava de Atium",
    };
    private string[] descripcionMetalFeru = new string[] {
        // Brazal de Hierro
        "Forjado con cariño por un herrero novato.\n"+
        "Almacena tu peso y te ancla al suelo, permitiéndote resistir los empujes metálicos.",

        // Bracil de Acero
        "Su brillante superficie pulida vibra con energía.\n" +
        "Guarda tu velocidad y la libera cuando la necesitas, aumentando tu rapidez temporalmente.",

        // Esclava de Estaño
        "En su brillo tenue se puede leer el nombre \"Hildsar\".\n" +
        "Almacena tu percepción sensorial y la devuelve con intensidad, revelando caminos y objetos ocultos.",

        // Ajorca de Peltre
        "Grabada con los símbolos de los clubs de lucha clandestinos más famosos.\n" +
        "Conserva tu fuerza y la libera en un instante, otorgando un gran poder a tus ataques.",

        // Pulsera de Zinc
        "Con un diseño intrincado, elegante pero hasta cierto punto, simple.\n" +
        "Almacena tu energía mental, permitiendo reducir drásticamente el tiempo de reutilización de tus habilidades.",

        // Bracil de Latón
        "Un bracil de latón con un leve calor constante.\n" +
        "Guarda tu calma y carisma, haciendo que los enemigos pequeños te ignoren y facilitando la persuasión con NPCs.",

        // Brazal de Cobre
        "Un brazal de cobre, opaco y discreto.\n" +
        "Oculta tu presencia del mundo por un tiempo, permitiéndote moverte sin ser detectado.",

        // Pulsera de Bronce
        "Una pulsera de bronce con un extraño fulgor.\n" +
        "Almacena tu atención a los detalles, revelando la ubicación de enemigos que intentan ocultarse.",

        // Brazalete de Cadmio
        "Un brazalete de cadmio, frío al tacto.\n" +
        "Retiene el flujo del tiempo a tu alrededor, permitiéndote congelar el mundo por un instante.",

        // Bracil de Bendaleo
        "Un bracil de bendaleo con una forma fluida.\n" +
        "Almacena tu estabilidad y la desata al instante, permitiéndote recuperarte rápidamente de cualquier aturdimiento.",

        // Ajorca de Oro
        "Una ajorca de oro con un brillo sanador.\n" +
        "Guarda tu vitalidad y la devuelve cuando la necesites, restaurando tu salud.",

        // Brazalete de Electro
        "Un brazalete de electro que emana una sensación de abundancia.\n" +
        "Almacena la suerte en la obtención de recursos, permitiéndote duplicar lo que consigues.",

        // Brazal de Cromo
        "Una escarpia de cromo con un diseño absorbente.\n" +
        "Drena la energía almacenada en los demás, vaciando sus reservas en un instante.",

        // Esclava de Nicrosil
        "Un clavo de nicrosil que parece vibrar con poder.\n" +
        "Desata una explosión de energía que aturde a todos los enemigos cercanos.",

        // Brazalete de Aluminio
        "Un tornillo de aluminio con un brillo apagado.\n" +
        "Cancela todas las alteraciones en tu mente, restaurando tu estabilidad y reiniciando el ciclo de locura.",

        // Pulsera de Duraluminio
        "Una alcayata de duraluminio de aspecto resistente.\n" +
        "Intensifica tus habilidades alománticas, permitiéndote desatar su máximo potencial.",

        // Esclava de Atium
        "Una escarpia de atium, extremadamente rara y valiosa.\n" +
        "Guarda un fragmento de una existencia ajena, otorgándote una segunda oportunidad al borde de la muerte."
    };
    private string[] nombreMetalHema = new string[] {
        "Clavo de Hierro",
        "Tornillo de Acero",
        "Tachuela de Estaño",
        "Punta de Peltre",
        "Tachuela de Zinc",
        "Escarpia de Latón",
        "Clavo de Cobre",
        "Alcayata de Bronce",
        "Tornillo de Cadmio",
        "Punta de Bendaleo",
        "Tachuela de Oro",
        "Alcayata de Electro",
        "Escarpia de Cromo",
        "Clavo de Nicrosil",
        "Tornillo de Aluminio",
        "Alcayata de Duraluminio",
        "Escarpia de Atium",
    };
    private string[] descripcionMetalHema = new string[] {
        //Clavo de Hierro
        "Extraído del pie de un padre skaa crucificado buscando a su hijo.\n"+
        "Impregnado de su fortaleza, otorga resistencia a los empujes.",

        //Tornillo de Acero
        "Arrebatado del cráneo de un prestigioso ladrón.\n"+
        "Imbuido con su presteza, otorga velocidad de movimiento.",

        //Tachuela de Estaño
        "Quitada del costado de una recolectora de objetos valiosos.\n"+
        "Pringada de su agudeza, permite ver objetos y caminos secretos",

        //Punta de Peltre
        "Sacada de la cuenca de un violento sin más.\n"+
        "Empapada de su volatilidad, potencia los ataques.",

        //Tachuela de Zinc
        "Extirpada con tristeza del cuello de un niño perdido por las calles.\n"+
        "Bañada en su adaptabilidad, reduce el tiempo de reutilización de los objetos",

        //Escarpia de Latón
        "Arrancada de la sien de un pirómano empedernido.\n"+
        "Manchada por su amor al fuego, reduce el daño de este.",

        //Clavo de Cobre
        "Retirado con respeto del pecho de un remunerado cartógrafo.\n"+
        "Haciendo acopio de su saber, muestra en el mapa los puntos más interesantes.",

        //Alcayata de Bronce
        "Arrebatada de la frente de un matasanos desquiciado.\n"+
        "Embebida de su conocimiento, muestra la salud de los enemigos.",

        //Tornillo de Cadmio
        "Sacado del pulmón de un minero en las cuevas de carbón.\n"+
        "Imbuido de su capacidad, aumenta el tiempo que se puede aguantar la respiración.",

        //Punta de Bendaleo
        "Quitada del hombro de un pescador mecido por las mareas.\n"+
        "Empapada de su sino, aumenta la cantidad de recursos conseguidos.",

        //Tachuela de Oro
        "Extraída del antebrazo de un aclamado doctor sin título.\n"+
        "Bañada en su saber, otorga una curación lenta pero sin pausa",

        //Alcayata de Electro
        "Arrancada con rabia de la espalda de un Inquisidor.\n"+
        "Manchada de su inhumanidad, mejora los efectos hemalúrgicos",

        //Escarpia de Cromo
        "Retirada de la mano un ilusionista de un noble.\n"+
        "Imbuida con su naturaleza escurridiza, otorga inmunidad temporal a los efectos negativos.",

        //Clavo de Nicrosil
        "Sacado del omóplato de un vidente sin oficio ni beneficio.\n"+
        "Embebido de su mala saña, aumenta el tiempo de efectos negativos de los enemigos.",

        //Tornillo de Aluminio
        "Quitado del abdomen de un embalsamador altruista.\n"+
        "Impregando de su resiliencia, acelera la desaparición de efectos negativos.",

        //Alcayata de Duraluminio
        "Extirpada del brazo de un guardaespaldas violento sin familia.\n"+
        "Pringada de su fuerza, permite concentrar ataques más potentes.",

        //Escarpia de Atium
        "Arrancada con saña de la pierna de un Inquisidor.\n"+
        "Manchada de su potencia, otorga la probabilidad de esquivar cualquier ataque.",
    };

    public GameObject pauseMenu;
    public bool isPaused = false;

    public GameObject[] menuPanels;
    private int currentPanelIndex = 0;

    public float transitionDuration = 0.15f;

    private int selectedMetalIndex = 0;
    private int selectedMetalIndexAlo = 0;
    private int selectedMetalIndexFeru = 0;
    private int selectedMetalIndexHema = 0;
    private GameObject grupoMetal;

    private bool canChangeMetal = true;
    private bool canChangeObjeto = true;

    private bool inMetales = false;

    private bool inMetalPage = false;
    private bool[] inWhatMetalPage = {false, false, false};

    public TextMeshProUGUI txtNombreAlomancia;
    public TextMeshProUGUI txtDescripcionAlomancia;
    public TextMeshProUGUI txtNombreFeruquimia;
    public TextMeshProUGUI txtDescripcionFeruquimia;
    public TextMeshProUGUI txtNombreHemalurgia;
    public TextMeshProUGUI txtDescripcionHemalurgia;
    public TextMeshProUGUI txtEstado;
    public TextMeshProUGUI txtFase;

    public TextMeshProUGUI txtNombreAlo;
    public TextMeshProUGUI txtDescripcionAlo;
    public TextMeshProUGUI txtAccion;
    public TextMeshProUGUI txtPosicion;
    public TextMeshProUGUI txtTipo;


    PlayerScript playerScript;
    public Dictionary<int, int> slotMetalAlo = new Dictionary<int, int>();
    public Dictionary<int,int> slotMetalFeru = new Dictionary<int, int>();
    public Dictionary<int, int> slotMetalHema = new Dictionary<int, int>();
    public Image[] slotsHemaImages;
    public Sprite[] spritesHema;

    private int selectedIndexObjetosX = 0;
    private int selectedIndexObjetosY = 0;
    public FilaImagenes[] objetos;
    public Sprite selectedFrame;
    public Sprite notSelectedFrame;
    public Sprite[] objetosSprite;

    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        if (playerScript == null) {
            Debug.LogError("No se encontró el PlayerScript en la escena.");
            return;
        }

        pauseMenu.SetActive(false);

        for(int i = 0; i < menuPanels.Length; i++){
            menuPanels[i].SetActive(false);
        }
        menuPanels[0].SetActive(true);
    }

    public void Update(){
        slotMetalAlo = playerScript.slotMetalAlo;
        slotMetalFeru = playerScript.slotMetalFeru;
        slotMetalHema = playerScript.slotMetalHema;
        DetectMetalPage();
        isDatos();
        SetFraseHemalurgia();
        UpdateSelection();
        UpdateObjetos();
    }

    private void SetFraseHemalurgia(){
        int sh = DatabaseManager.Instance.GetInt($"SELECT COUNT(*) FROM metal_archivo WHERE archivo_id = 1 AND slot_h != 0;");
        string text = "";
        switch (sh){
            case 0:
                text = "Puro de Espíritu";
                break;
            case 1:
                text = "El Primer Clavo";
                break;
            case 2:
            case 3:
                text = "Rozando el Umbral";
                break;
            case 4:
                text = "Marcado por el Acero";
                break;
            case 5:
            case 6:
                text = "Forjado en Sangre";
                break;
            case 7:
                text = "Abrázame, Ruina";
                break;
            default:
                text = "��";
                break;
        }
        txtEstado.text = text;

        for(int i = 1; i <= 7; i++){
            if(slotMetalHema[i] != 0){
                slotsHemaImages[i-1].gameObject.SetActive(true);
                slotsHemaImages[i-1].sprite = spritesHema[slotMetalHema[i]-1];
                if(slotMetalHema[i] == selectedMetalIndex + 1){
                    slotsHemaImages[i-1].color = Color.yellow;
                }else{
                    slotsHemaImages[i-1].color = Color.white;
                }
            }else{
                slotsHemaImages[i-1].gameObject.SetActive(false);
            }
        }
    }

    private void DetectMetalPage(){
        GameObject panelActual = menuPanels[currentPanelIndex];

        inWhatMetalPage[0] = panelActual.name == "PanelAlomancia";
        inWhatMetalPage[1] = panelActual.name == "PanelFeruquimia";
        inWhatMetalPage[2] = panelActual.name == "PanelHemalurgia";

        inMetalPage = inWhatMetalPage[0] || inWhatMetalPage[1] || inWhatMetalPage[2];
    }

    public void isDatos(){
        GameObject panelActual = menuPanels[currentPanelIndex];
        if (inMetalPage){
            inMetales = true;
            Transform imagenesMetales = panelActual.transform.Find("ImagenesMetales");
            if (imagenesMetales == null){
                return;
            }

            Transform grupoTransform = imagenesMetales.Find("Metales");
            if (grupoTransform == null){
                return;
            }

            grupoMetal = grupoTransform.gameObject;

            for (int j = 0; j < grupoMetal.transform.childCount; j++){
                bool desbloqueado = (bool)DatabaseManager.Instance.ExecuteScalar(
                    $"SELECT desbloqueado_{panelActual.name[5].ToString().ToLower()} FROM metal_archivo WHERE archivo_id = 1 AND metal_id = {j + 1};"
                );

                Debug.Log($"panelActual.name[5].ToString().ToLower(): {panelActual.name[5].ToString().ToLower()} - {desbloqueado}");

                GameObject metal = grupoMetal.transform.GetChild(j).gameObject;
                metal.SetActive(desbloqueado);

            }
        }else{
            inMetales = false;
        }
    }

    public void OnPause(InputAction.CallbackContext context){
        if (context.action.triggered){
            if (isPaused){
                ResumeGame();
            }else{
                PauseGame();
            }
        }
    }

    public void onMenuRight(InputAction.CallbackContext context){
        if (context.action.triggered && isPaused){
            ChangePanel(1);
        }
    }

    public void onMenuLeft(InputAction.CallbackContext context){
        if (context.action.triggered && isPaused){
            ChangePanel(-1);
        }
    }

    public void OnMoveLeft(InputAction.CallbackContext context){
        DetectMetalPage();
        if (context.performed && isPaused && inMetalPage){
            ChangeSelectedMetal(-1);
        }else{
            ChangeSelectedObjeto(-1,0);
        }
    }

    public void OnMoveRight(InputAction.CallbackContext context){
        DetectMetalPage();
        if (context.performed && isPaused && inMetalPage){
            ChangeSelectedMetal(1);
        }else{
            ChangeSelectedObjeto(1,0);
        }
    }

    public void OnMoveUp(InputAction.CallbackContext context){
        if (context.performed && isPaused){
            ChangeSelectedObjeto(0,-1);
        }
    }

    public void OnMoveDown(InputAction.CallbackContext context){
        if (context.performed && isPaused){
            ChangeSelectedObjeto(0,1);
        }
    }

    private void ChangeSelectedObjeto(int deltaX, int deltaY){
        if(!isPaused) return;
        if (!canChangeObjeto) {
            Debug.LogWarning("Cambio de metal bloqueado temporalmente.");
            return;
        }
        if (objetos == null || objetos.Length == 0) return;

        int maxY = objetos.Length;
        int maxX = objetos[selectedIndexObjetosY].columnas.Length;

        selectedIndexObjetosY = Mathf.Clamp(selectedIndexObjetosY + deltaY, 0, maxY - 1);

        maxX = objetos[selectedIndexObjetosY].columnas.Length;

        selectedIndexObjetosX = Mathf.Clamp(selectedIndexObjetosX + deltaX, 0, maxX - 1);

        Image imagenSeleccionada = objetos[selectedIndexObjetosY].columnas[selectedIndexObjetosX];
        
        foreach(FilaImagenes f in objetos){
            foreach(Image i in f.columnas){
                i.sprite = notSelectedFrame;
            }
        }
        imagenSeleccionada.sprite = selectedFrame;
        
        canChangeObjeto = false;
        StartCoroutine(ResetChangeObjetoCooldown());

        return;
    }

    private IEnumerator ResetChangeObjetoCooldown(){
        yield return new WaitForSecondsRealtime(0.1f);
        canChangeObjeto = true;
    }

    private void UpdateObjetos(){
        List<Objeto> objetosDes = playerScript.objetosDesbloqueados;
        int spriteIndex = 0;

        for (int fila = 0; fila < objetos.Length; fila++){
            for (int col = 0; col < objetos[0].columnas.Length; col++){
                Image img = objetos[fila].columnas[col].transform.GetChild(0).GetComponent<Image>();

                if (spriteIndex < objetosDes.Count){
                    Objeto obj = objetosDes[spriteIndex];
                    if (obj.id >= 0 && obj.id < objetosSprite.Length){
                        img.sprite = objetosSprite[obj.id-1];
                        img.color = new Color(1, 1, 1, 1);
                    }else{
                        img.sprite = null;
                        img.color = new Color(1, 1, 1, 0);
                    }
                    spriteIndex++;
                }else{
                    img.sprite = null;
                    img.color = new Color(1, 1, 1, 0);
                }
            }
        }
    }

    public void OnSelect(InputAction.CallbackContext context){
        if (context.performed && isPaused){
            Select();
        }
    }

    private void Select() {
        if (inMetalPage && selectedMetalIndex >= 0) {
            Dictionary<int, int> diccionarioActual = null;
            int maxSlots = 0;

            // Determinar qué diccionario usar y el número máximo de slots
            if (inWhatMetalPage[0]) {
                diccionarioActual = slotMetalAlo;
                maxSlots = 8;
                Debug.Log("Probando como Alo");
            } else if (inWhatMetalPage[1]) {
                diccionarioActual = slotMetalFeru;
                maxSlots = 4;
                Debug.Log("Probando como Feru");
            } else if (inWhatMetalPage[2]) {
                diccionarioActual = slotMetalHema;
                maxSlots = 7;
                Debug.Log("Probando como Hema");
            }else{
                return;
            }

            if (diccionarioActual == null) {
                Debug.LogWarning("No se ha podido determinar el tipo de metal.");
                return;
            }

            // Buscar si el metal ya está asignado a algún slot
            int slotUsado = diccionarioActual.FirstOrDefault(kv => kv.Value == selectedMetalIndex + 1).Key;

            Debug.Log("slotUsado: " + slotUsado);

            if (slotUsado > 0) {
                Debug.Log("El metal está siendo usado");
                diccionarioActual[slotUsado] = 0; // Lo quitamos del slot
                Debug.Log("Quitado del set");
            } else {
                Debug.Log("El metal NO está siendo usado");
                for (int i = 1; i <= maxSlots; i++) {
                    if (!diccionarioActual.ContainsKey(i) || diccionarioActual[i] == 0) {
                        diccionarioActual[i] = selectedMetalIndex + 1; // Lo asignamos al primer slot libre
                        Debug.Log("Encontrado slot libre: " + i);
                        Debug.Log("Añadido al set");
                        break;
                    }
                }
            }
        }
        
        // foreach(var metal in slotMetalHema.Keys){
        //     Debug.Log($"SELECT METAL PAUSE MANAGER {metal}: {slotMetalHema[metal]}");
        // }

        UpdateSelection();
    }


    private void ChangeSelectedMetal(int direction){
        if (!canChangeMetal || grupoMetal == null) {
            Debug.LogWarning("Cambio de metal bloqueado temporalmente.");
            return;
        }

        int totalMetales = grupoMetal.transform.childCount;
        int newIndex;

        if (inWhatMetalPage[0]) { // Alomancia
            newIndex = selectedMetalIndexAlo;
        } else if (inWhatMetalPage[1]) { // Feruquimia
            newIndex = selectedMetalIndexFeru;
        } else if (inWhatMetalPage[2]) { // Hemalurgia
            newIndex = selectedMetalIndexHema;
        } else {
            return;
        }

        for (int i = 1; i < totalMetales; i++){
            newIndex = (newIndex + direction + totalMetales) % totalMetales;

            if (grupoMetal.transform.GetChild(newIndex).gameObject.activeSelf){
                if (inWhatMetalPage[0]) {
                    selectedMetalIndexAlo = newIndex;
                } else if (inWhatMetalPage[1]) {
                    selectedMetalIndexFeru = newIndex;
                } else if (inWhatMetalPage[2]) {
                    selectedMetalIndexHema = newIndex;
                }

                UpdateSelection();

                canChangeMetal = false;
                StartCoroutine(ResetChangeCooldown());

                return;
            }
        }
    }

    private IEnumerator ResetChangeCooldown(){
        yield return new WaitForSecondsRealtime(0.2f);
        canChangeMetal = true;
    }

    private IEnumerator TransitionPanels(GameObject oldPanel, GameObject newPanel){
        newPanel.SetActive(true);
        oldPanel.SetActive(false);

        yield return null;
    }

    private void UpdateSelection(){
        if (inWhatMetalPage[0]) {
            selectedMetalIndex = selectedMetalIndexAlo;
        } else if (inWhatMetalPage[1]) {
            selectedMetalIndex = selectedMetalIndexFeru;
        } else if (inWhatMetalPage[2]) {
            selectedMetalIndex = selectedMetalIndexHema;
        }
        if (grupoMetal == null) return;
        if (!inMetalPage) return;

        for (int i = 1; i <= grupoMetal.transform.childCount; i++){
            GameObject metal = grupoMetal.transform.GetChild(i-1).gameObject;
            UnityEngine.UI.Image img = metal.GetComponent<UnityEngine.UI.Image>();

            int slotOcupado = -1;
            foreach (var kv in inWhatMetalPage[0]? slotMetalAlo : inWhatMetalPage[1]? slotMetalFeru : inWhatMetalPage[2]? slotMetalHema : null) {
                if (kv.Value == i) {
                    slotOcupado = kv.Key;
                    break;
                }
            }

            if (img != null){
                if(i-1 == selectedMetalIndex){
                    if(slotOcupado > 0){
                        img.color = Color.red;
                    }else{
                        img.color = Color.magenta;
                    }
                }else{
                    if(slotOcupado > 0){
                        img.color = Color.yellow;
                    }else{
                        img.color = Color.white;
                    }
                }
            }else{
                Debug.LogWarning($"El metal {metal.name} no tiene Image asignado.");
            }
        }

        GameObject panelActual = menuPanels[currentPanelIndex];

        if (inWhatMetalPage[0]){
            if (txtAccion != null && txtPosicion != null && txtTipo != null){
                txtNombreAlo.text = nombreMetalAlo[selectedMetalIndex];
                txtDescripcionAlo.text = descripcionMetalAlo[selectedMetalIndex];

                txtAccion.text = selectedMetalIndex%2 == 0 ? "Tirón" : "Empuje";

                txtPosicion.text = (selectedMetalIndex % 4 == 1 || selectedMetalIndex % 4 == 0) ? "Externo" : "Interno";

                txtTipo.text =
                (selectedMetalIndex % 16 == 0 || selectedMetalIndex % 16 == 1 || selectedMetalIndex % 16 == 2 || selectedMetalIndex % 16 == 3) ? "Físico" :
                (selectedMetalIndex % 16 == 4 || selectedMetalIndex % 16 == 5 || selectedMetalIndex % 16 == 6 || selectedMetalIndex % 16 == 7) ? "Mental" :
                (selectedMetalIndex % 16 == 8 || selectedMetalIndex % 16 == 9 || selectedMetalIndex % 16 == 10 || selectedMetalIndex % 16 == 11) ? "Temporal" :
                "Espiritual";
            }
        }else if (inWhatMetalPage[1]){
            if (txtNombreFeruquimia != null && txtDescripcionFeruquimia != null){
                txtNombreFeruquimia.text = nombreMetalFeru[selectedMetalIndex];
                txtDescripcionFeruquimia.text = descripcionMetalFeru[selectedMetalIndex];
            }
        }else if(inWhatMetalPage[2]){
            if (txtNombreHemalurgia != null && txtDescripcionHemalurgia != null){
                txtNombreHemalurgia.text = nombreMetalHema[selectedMetalIndex];
                txtDescripcionHemalurgia.text = descripcionMetalHema[selectedMetalIndex];
            }
        }
    }

    private void ChangePanel(int direction){
        if (menuPanels.Length == 0) return;

        int previousPanelIndex = currentPanelIndex;
        currentPanelIndex = (currentPanelIndex + direction + menuPanels.Length) % menuPanels.Length;

        StartCoroutine(TransitionPanels(menuPanels[previousPanelIndex], menuPanels[currentPanelIndex]));
        
        UpdateSelection();
    }

    public void PauseGame(){
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        StartCoroutine(FadeAudioListener(0.3f,null));
    }

    public void ResumeGame(){
        isPaused = false;
        StartCoroutine(FadeAudioListener(1f, () =>{
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }));
    }

    private IEnumerator FadeAudioListener(float targetVolume, System.Action onComplete){
        float startVolume = AudioListener.volume;
        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration){
            elapsedTime += Time.unscaledDeltaTime;
            AudioListener.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
            yield return null;
        }

        AudioListener.volume = targetVolume;

        onComplete?.Invoke();
    }

    public void QuitGame(){
        Time.timeScale = 1f;
        Application.Quit();
    }
}

[System.Serializable]
public class FilaImagenes {
    public Image[] columnas;
}