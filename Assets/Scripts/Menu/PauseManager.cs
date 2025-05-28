using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

public class PauseManager : MonoBehaviour{

    GameObject panelActual;

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
        "Pulsera de Duralumín",
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
        "???",
        // "Almacena tu energía mental, permitiendo reducir drásticamente el tiempo de reutilización de tus habilidades.",

        // Bracil de Latón
        "Un bracil de latón con un leve calor constante.\n" +
        "Guarda tu calma y carisma, haciendo que los enemigos pequeños te ignoren y facilitando la persuasión con NPCs.",

        // Brazal de Cobre
        "Entregado por el bibliotecario, su diseño discreto y digno guarda similitud con aquel que la otrogó.\n" +
        "Almacena recuerdos y conocimiento, pudiendo mantenerlos intactos hasta que se decanten",

        // Pulsera de Bronce
        "Una pulsera de bronce con un extraño fulgor.\n" +
        "???",
        // "Almacena tu atención a los detalles, revelando la ubicación de enemigos que intentan ocultarse.",

        // Brazalete de Cadmio
        "Un brazalete de cadmio, frío al tacto.\n" +
        "Guarda alienta, obligando a tomar aire con asiduidad, cuando se es decantado no se requiere respirar con tanta frecuencia.",

        // Bracil de Bendaleo
        "Un bracil de bendaleo con una forma fluida.\n" +
        "???",
        // "Almacena tu estabilidad y la desata al instante, permitiéndote recuperarte rápidamente de cualquier aturdimiento.",

        // Ajorca de Oro
        "Una ajorca de oro con un brillo sanador.\n" +
        "Mientras se esté guardando, el portador se sentirá débil y enfermo, al extraer su contenido las heridas sanarán más rápido.",

        // Brazalete de Electro
        "Un brazalete de electro que emana una sensación de abundancia.\n" +
        "Cuando se esté almacenando, cualquier empujón puede hacer perder el equilibrio, pero más difícil será perderlo cuando se decante.",

        // Brazal de Cromo
        "Un brazal de cromo con un diseño absorbente.\n" +
        "La fortuna y copiosidad de lo encontrado se verá reducida mientras se esté guardando, mientras que aumentará al extraerla.",

        // Esclava de Nicrosil
        "Un clavo de nicrosil que parece vibrar con poder.\n" +
        "La alomancia será afectada negativamente mientras se almacene, pero más poderosa cuando se educe.",

        // Brazalete de Aluminio
        "Un tornillo de aluminio con un brillo apagado.\n" +
        "Al guardarse, la identidad propia se verá absorbida, permitiendo crear mentes de metal sin dueño pero facilita el influjo de la hemalurgia, "+
        "al decantarse, el efecto de la hemalurgia se revertirá.",

        // Pulsera de Duralumín
        "Una alcayata de duraluminio de aspecto resistente.\n" +
        "La conexión que se pueda tener con el entorno se reducirá al guardarse, aumentando la hostilidad de los transeuntes, mientras que al extraerse hará que sean más cercanos.",

        // Esclava de Atium
        "Una escarpia de atium, extremadamente rara y valiosa.\n" +
        "Guarda la vida que te queda aún por vivir sin efecto aparente, pero al decantarla, puede llegar a salvarte de una muerte segura."
        // "Guarda un fragmento de una existencia ajena, otorgándote una segunda oportunidad al borde de la muerte."
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
        "Alcayata de Duralumín",
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

        //Alcayata de Duralumín
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

    private int selectedIndex = 0;
    private int selectedMetalIndexAlo = 0;
    private int selectedMetalIndexFeru = 0;
    private int selectedMetalIndexHema = 0;
    private int selectedMetalVialIndex = 0;
    private int selectedRawMetalVialIndex = 0;

    private GameObject grupoMetal;

    private bool canChangeMetal = true;
    private bool canChangeObjeto = true;

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

    public Image[] slotsHemaImages;
    public Sprite[] spritesHema;

    private int selectedIndexObjetosX = 0;
    private int selectedIndexObjetosY = 0;
    public FilaImagenes[] objetos;
    public Sprite selectedFrame;
    public Sprite notSelectedFrame;
    public Sprite[] objetosSprite;

    public GameObject rellenos;
    
    public Sprite[] vialesMetalesSprites;
    public GameObject Viales;
    public GameObject Metales;
    public GameObject Detalles;

    Dictionary<string, string> coloresMetales = new Dictionary<string, string>(){
        {"Hierro", "#4B4B4B"},
        {"Acero", "#A9A9A9"},
        {"Estaño", "#708090"},
        {"Peltre", "#B0C4DE"},
        {"Zinc", "#808000"},
        {"Latón", "#DAA520"},
        {"Cobre", "#B87333"},
        {"Bronce", "#CD7F32"},
        {"Cadmio", "#6A5ACD"},
        {"Bendaleo", "#9370DB"},
        {"Oro", "#FFD700"},
        {"Electro", "#FFB700"},
        {"Cromo", "#C0C0C0"},
        {"Nicrosil", "#D3D3D3"},
        {"Aluminio", "#ADD8E6"},
        {"Duralumín", "#4682B4"},
        {"Atium", "#5EF2DC"},
    };

    PlayerData pd;

    void Start(){
        panelActual = menuPanels[currentPanelIndex];
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
        Detalles.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void Update(){
        pd = playerScript.pd;
        panelActual = menuPanels[currentPanelIndex];

        DetectMetalPage();
        isDatos();
        UpdateHemalurgy();
        UpdateSelection();
        UpdateInventory();
        UpdateMetales();
    }

    private void UpdateMetales(){
        for(int i = 0; i < rellenos.transform.childCount; i++){
            Image relleno = rellenos.transform.GetChild(i).GetComponent<Image>();

            string metal = playerScript.GetNombreMetal(i+1);

            float cant = pd.GetMetalById(i+1).GetAmount();
            float cap = pd.GetMetalById(i+1).GetCapacity();
            
            float fillAmount = cant / cap;
            if(i!=16){
                fillAmount/=8;
            }
            
            Color color;
            if (ColorUtility.TryParseHtmlString(coloresMetales[metal], out color)) {
                relleno.color = color;
            }

            relleno.fillAmount = fillAmount;
        }
    }

    private void UpdateHemalurgy(){
        string text = "";
        switch (pd.GetNailNum()){
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
        switch (pd.GetPhase()){
            case "silencio":
                text = "Silencio en la Bruma";
                break;
            case "murmullos":
                text = "Murmullos de las Paredes";
                break;
            case "ojos":
                text = "Ojos en la Oscuridad";
                break;
            case "mundo":
                text = "El Mundo se Rompe";
                break;
            case "dominio":
                text = "Dominio de Ruina";
                break;
            case "extasis":
                text = "Éxtasis Hemalúrgico";
                break;
            case "declive":
                text = "Declive Metálico";
                break;
            default:
                text = "��";
                break;
        }
        txtFase.text = text;

        for(int i = 1; i <= pd.GetHemaSlots().Length; i++){
            HemaMetal hm = pd.GetHemaMetalInSlot(i);
            if(hm != null){
                slotsHemaImages[i-1].gameObject.SetActive(true);
                slotsHemaImages[i-1].sprite = spritesHema[(int)hm.GetMetal()-1];
                if(selectedMetalIndexHema+1 == (int)hm.GetMetal()){
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
                bool desbloqueado = (inWhatMetalPage[0] ? pd.GetUnlockedAloMetals() : 
                                    inWhatMetalPage[1] ? pd.GetUnlockedFeruMetals() : 
                                    inWhatMetalPage[2] ? pd.GetUnlockedHemaMetals() : null )[j] == 1;

                GameObject metal = grupoMetal.transform.GetChild(j).gameObject;
                metal.SetActive(desbloqueado);

            }
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
        }else if (context.performed && isPaused && panelActual.name == "PanelMetales"){
            ChangeVial(-1);
        }else if (context.performed && isPaused && panelActual.name == "PanelInventario"){
            ChangeSelectedObject(-1,0);
        }
    }
    public void OnMoveRight(InputAction.CallbackContext context){
        DetectMetalPage();
        if (context.performed && isPaused && inMetalPage){
            ChangeSelectedMetal(1);
        }else if (context.performed && isPaused && panelActual.name == "PanelMetales"){
            ChangeVial(1);
        }else if (context.performed && isPaused && panelActual.name == "PanelInventario"){
            ChangeSelectedObject(1,0);
        }
    }
    public void OnMoveUp(InputAction.CallbackContext context){
        if (context.performed && isPaused && panelActual.name == "PanelInventario"){
            ChangeSelectedObject(0,-1);
        }
    }
    public void OnMoveDown(InputAction.CallbackContext context){
        if (context.performed && isPaused && panelActual.name == "PanelInventario"){
            ChangeSelectedObject(0,1);
        }
    }

    private void ChangeSelectedObject(int deltaX, int deltaY){
        if (!isPaused) return;
        if (!canChangeObjeto) {
            return;
        }
        if (objetos == null || objetos.Length == 0) return;

        int maxY = objetos.Length;
        int maxX = objetos[selectedIndexObjetosY].columnas.Length;

        selectedIndexObjetosY = (selectedIndexObjetosY + deltaY + maxY) % maxY;

        maxX = objetos[selectedIndexObjetosY].columnas.Length;

        selectedIndexObjetosX = (selectedIndexObjetosX + deltaX + maxX) % maxX;

        Image imagenSeleccionada = objetos[selectedIndexObjetosY].columnas[selectedIndexObjetosX];

        foreach (FilaImagenes f in objetos){
            foreach (Image i in f.columnas){
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

    private Consumible[,] consumibleGrid;

    private void UpdateInventory() {
        int totalSlots = 12;
        int vialMetalCount = 8;
        int laudanumIndex = 8;
        int projectileStartIndex = 9;

        // Obtén las listas de consumibles desde pd
        Consumible[] vialesMetales = (Consumible[])pd.GetMetalVials();
        Consumible laudanum = pd.GetLaudano();
        Consumible[] proyectiles = (Consumible[])pd.GetProjectiles();

        int filas = objetos.Length;
        int columnas = objetos[0].columnas.Length;

        consumibleGrid = new Consumible[filas, columnas];

        int slotIndex = 0;

        for (int fila = 0; fila < filas; fila++) {
            for (int col = 0; col < columnas; col++) {
                Image img = objetos[fila].columnas[col].transform.GetChild(0).GetComponent<Image>();
                Consumible asignado = null;

                if (slotIndex < vialMetalCount) {
                    if (slotIndex < vialesMetales.Length && vialesMetales[slotIndex] != null) {
                        img.sprite = vialesMetales[slotIndex].GetSprite();
                        img.color = Color.white;
                        asignado = vialesMetales[slotIndex];
                    } else {
                        img.sprite = null;
                        img.color = new Color(1, 1, 1, 0);
                    }
                } else if (slotIndex == laudanumIndex) {
                    if (laudanum != null) {
                        img.sprite = laudanum.GetSprite();
                        img.color = Color.white;
                        asignado = laudanum;
                    } else {
                        img.sprite = null;
                        img.color = new Color(1, 1, 1, 0);
                    }
                } else if (slotIndex >= projectileStartIndex && slotIndex < totalSlots) {
                    int projectileIndex = slotIndex - projectileStartIndex;
                    if (projectileIndex < proyectiles.Length && proyectiles[projectileIndex] != null) {
                        img.sprite = proyectiles[projectileIndex].GetSprite();
                        img.color = Color.white;
                        asignado = proyectiles[projectileIndex];
                    } else {
                        img.sprite = null;
                        img.color = new Color(1, 1, 1, 0);
                    }
                } else {
                    img.sprite = null;
                    img.color = new Color(1, 1, 1, 0);
                }

                consumibleGrid[fila, col] = asignado;
                SetData(GetConsumibleSeleccionado());
                slotIndex++;
            }
        }
    }

    public GameObject InfoInventario;

    void SetData(Consumible consu){
        if(consu == null) return;
        InfoInventario.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = consu.GetName();
        InfoInventario.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = consu.GetDescription();
    }
    
    public Consumible GetConsumibleSeleccionado() {
        if (consumibleGrid == null) return null;
        return consumibleGrid[selectedIndexObjetosY, selectedIndexObjetosX];
    }

    public void OnSelect(InputAction.CallbackContext context){
        if (context.performed && isPaused && panelActual.name == "PanelInventario" ){
            SelectInventory();
        }else if (context.performed && isPaused){
            Select();
        }
    }

    void SelectInventory(){
        pd.EquipItem(GetConsumibleSeleccionado());
    }

    private void Select() {
        if(inMetalPage && selectedIndex >= 0){
            if (inWhatMetalPage[0]) {
                pd.EquipAloMetal(selectedIndex+1);
            } else if (inWhatMetalPage[1]) {
                pd.EquipFeruMetal(selectedIndex+1);
            } else if (inWhatMetalPage[2]) {
                pd.EquipHemaMetal(selectedIndex+1);
            }
        } else if (panelActual.name == "PanelMetales" && selectedIndex >= 0) {
                HandleSelectMetalVial();
            }

        UpdateSelection();
    }


    private void ChangeSelectedMetal(int direction){
        if (!canChangeMetal || grupoMetal == null) {
            return;
        }

        int totalMetales = grupoMetal.transform.childCount;
        int newIndex;

        if (inWhatMetalPage[0]) {
            newIndex = selectedMetalIndexAlo;
        } else if (inWhatMetalPage[1]) {
            newIndex = selectedMetalIndexFeru;
        } else if (inWhatMetalPage[2]) {
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

    bool selectedVial = false;

    private void ChangeVial(int direction){
        if (!canChangeMetal) {
            return;
        }

        int opciones;
        int newIndex;

        if (panelActual.name == "PanelMetales" && !selectedVial) {
            opciones = Viales.transform.GetChild(0).childCount;
            newIndex = selectedMetalVialIndex;

        }else if (panelActual.name == "PanelMetales" && selectedVial) {
            opciones = Metales.transform.GetChild(0).childCount;
            newIndex = selectedRawMetalVialIndex;

        }else{
            return;
        }

        for (int i = 0; i < opciones; i++) {
            newIndex = (newIndex + direction + opciones) % opciones;

            if (panelActual.name == "PanelMetales" && !selectedVial) {
                if (Viales.transform.GetChild(0).transform.GetChild(newIndex).gameObject.activeSelf) {
                    selectedMetalVialIndex = newIndex;

                    UpdateSelection();

                    canChangeMetal = false;
                    StartCoroutine(ResetChangeCooldown());

                    return;
                }
            } else if (panelActual.name == "PanelMetales" && selectedVial) {
                if (Metales.transform.GetChild(0).transform.GetChild(newIndex).gameObject.activeSelf) {
                    selectedRawMetalVialIndex = newIndex;

                    UpdateSelection();

                    canChangeMetal = false;
                    StartCoroutine(ResetChangeCooldown());

                    return;
                }
            }
        }
    }

    private IEnumerator TransitionPanels(GameObject oldPanel, GameObject newPanel){
        newPanel.SetActive(true);
        oldPanel.SetActive(false);

        yield return null;
    }

    int[] rawMetalsUsed = new int[17];

    int GetCapacityOfActualMetalVial(){
        int res = 0;
        foreach(int i in rawMetalsUsed){
            res += i;
        }

        return res;
    }

    void HandleSelectMetalVial(){
        if (!pd.GetMetalVialBySlot(selectedMetalVialIndex+1).roto && pd.GetMetalVialBySlot(selectedMetalVialIndex+1).content.Count == 0 && !selectedVial) {
            selectedVial = true;
            UpdateSelection();
            ShowNewMetalVialData();
        } else if(selectedVial) {
            MasMetal();
        }
    }

    void MasMetal(){
        rawMetalsUsed[selectedRawMetalVialIndex] += 20;
        if(rawMetalsUsed[selectedRawMetalVialIndex] > pd.GetRawMetalByMetalId(selectedRawMetalVialIndex+1).amount || GetCapacityOfActualMetalVial()>1000){
            rawMetalsUsed[selectedRawMetalVialIndex] -= 20;
        }
        ShowNewMetalVialData();
    }

    public void OnMenosMetal(InputAction.CallbackContext context){
        if (context.performed && isPaused && panelActual.name == "PanelMetales" && selectedVial){
            MenosMetal();
        }
    }

    void MenosMetal(){
        if(rawMetalsUsed[selectedRawMetalVialIndex] > 0){
            rawMetalsUsed[selectedRawMetalVialIndex] -= 20;
        }
        ShowNewMetalVialData();
    }

    void ShowNewMetalVialData(){
        for(int i = 0; i < rawMetalsUsed.Length; i++){
            string[] fisicos = {"Hierro","Acero","Estaño","Peltre"};
            string[] mentales = {"Zinc","Latón","Cobre","Bronce"};
            string[] temporales = {"Cadmio","Bendaleo","Oro","Electro"};
            string[] espirituales = {"Cromo","Nicrosil","Aluminio","Duralumín"};
            string[] atium = {"Atium"};

            string[][] nombre = {fisicos,mentales,temporales,espirituales,atium};
            if(i<16){
                Detalles.transform.GetChild(1).GetChild((i / 4)+1).GetChild(i % 4).GetComponent<TextMeshProUGUI>().text = $"{nombre[i/4][i%4]}: {rawMetalsUsed[i]}";
            }else{
                Detalles.transform.GetChild(1).GetChild(5).GetComponent<TextMeshProUGUI>().text = $"{nombre[i/4][i%4]}: {rawMetalsUsed[i]}";
            }
        }
    }

    public void OnCancelMetalVial(InputAction.CallbackContext context){
        if (context.performed && isPaused && panelActual.name == "PanelMetales" && selectedVial){
            CleanUsedRawMetals();
        }
    }

    public void OnAcceptMetalVial(InputAction.CallbackContext context){
        if (context.performed && isPaused && panelActual.name == "PanelMetales" && selectedVial){
            AcceptMetalVial();
        }
    }

    void AcceptMetalVial(){
        for(int i = 0; i < rawMetalsUsed.Length; i++){
            if(rawMetalsUsed[i] > 0){
                pd.AddRawMetalToMetalVial(selectedMetalVialIndex, i+1, rawMetalsUsed[i]);
                rawMetalsUsed[i] = 0;
            }
        }
        CleanUsedRawMetals();
    }

    void CleanUsedRawMetals(){
        for(int i = 0; i < rawMetalsUsed.Length; i++){
            rawMetalsUsed[i] = 0;
        }
        selectedRawMetalVialIndex = 0;
        selectedVial = false;
        UpdateSelection();
    }

    private void UpdateSelection(){
        if (inWhatMetalPage[0]) {
            selectedIndex = selectedMetalIndexAlo;

        } else if (inWhatMetalPage[1]) {
            selectedIndex = selectedMetalIndexFeru;

        } else if (inWhatMetalPage[2]) {
            selectedIndex = selectedMetalIndexHema;

        } else if (panelActual.name == "PanelMetales" && !selectedVial) {
            selectedIndex = selectedMetalVialIndex;

        } else if (panelActual.name == "PanelMetales" && !selectedVial) {
            selectedIndex = selectedRawMetalVialIndex;

        }

        if (panelActual.name == "PanelMetales" && !selectedVial) {
            Detalles.transform.GetChild(1).gameObject.SetActive(false);
            for (int i = 1; i <= Metales.transform.GetChild(0).childCount; i++) {
                Metales.transform.GetChild(0).GetChild(i - 1).GetComponent<Image>().color = Color.white;
            }
            for (int i = 1; i <= Viales.transform.GetChild(0).childCount; i++) {
                Image img = Viales.transform.GetChild(0).GetChild(i - 1).GetComponent<Image>();

                if(pd.GetMetalVialBySlot(i).roto){
                    img.sprite = vialesMetalesSprites[2];
                }else{
                    if (pd.GetMetalVialBySlot(i).content.Count == 0) {
                        img.sprite = vialesMetalesSprites[0];
                    } else {
                        img.sprite = vialesMetalesSprites[1];
                    }
                }

                if (img != null) {
                    if (i - 1 == selectedIndex) {
                        img.color = Color.red;

                        MetalVial mv = pd.GetMetalVialBySlot(i);
                        if(i!=17){
                            Detalles.transform.GetChild(0).GetChild(((i-1) % 2)).GetChild((i-1) / 2).GetComponent<TextMeshProUGUI>().color = Color.white;
                        }else{
                            Detalles.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.white;
                        }

                        int[] cantidades = new int[18];

                        foreach (MetalVialContent mvc in mv.content) {
                            cantidades[(int)mvc.metal] = mvc.amount;
                        }

                        string[] nombresIzquierda = {"Hierro","Estaño","Zinc","Cobre","Cadmio","Oro","Cromo","Aluminio"};
                        string[] nombresDerecha   = {"Acero","Peltre","Latón","Bronce","Bendaleo","Electro","Nicrosil","Duralumín"};

                        // Izquierda (impares 1, 3, 5, ...)
                        for (int j = 0; j < 8; j++) {
                            int metalIndex = 1 + j * 2;
                            string nombre = nombresIzquierda[j];
                            int cantidad = cantidades[metalIndex];

                            Detalles.transform.GetChild(0).GetChild(0).GetChild(j)
                                .GetComponent<TextMeshProUGUI>().color = cantidad > 0 ? Color.cyan : Color.white;

                            Detalles.transform.GetChild(0).GetChild(0).GetChild(j)
                                .GetComponent<TextMeshProUGUI>().text = $"{nombre}: {cantidad:0}";
                        }

                        // Derecha (pares 2, 4, 6, ...)
                        for (int j = 0; j < 8; j++) {
                            int metalIndex = 2 + j * 2;
                            string nombre = nombresDerecha[j];
                            int cantidad = cantidades[metalIndex];
                            
                            Detalles.transform.GetChild(0).GetChild(1).GetChild(j)
                                .GetComponent<TextMeshProUGUI>().color = cantidad > 0 ? Color.cyan : Color.white;

                            Detalles.transform.GetChild(0).GetChild(1).GetChild(j)
                                .GetComponent<TextMeshProUGUI>().text = $"{nombre}: {cantidad:0}";
                        }

                        // Atium (17)
                        int cantidadAtium = cantidades[17];
                            
                        Detalles.transform.GetChild(0).GetChild(2)
                            .GetComponent<TextMeshProUGUI>().color = cantidadAtium > 0 ? Color.cyan : Color.white;
                            
                        Detalles.transform.GetChild(0).GetChild(2)
                            .GetComponent<TextMeshProUGUI>().text = $"Atium: {cantidadAtium:0}";
                    } else {
                        img.color = Color.white;
                        if(i!=17){
                            TextMeshProUGUI txt = Detalles.transform.GetChild(0).GetChild(((i-1) % 2)).GetChild((i-1) / 2).GetComponent<TextMeshProUGUI>();
                            txt.color = txt.color == Color.cyan ? Color.cyan : Color.white;
                        }else{
                            TextMeshProUGUI txt = Detalles.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
                            txt.color = txt.color == Color.cyan ? Color.cyan : Color.white;
                        }
                    }
                }
            }
            return;
        }else if(panelActual.name == "PanelMetales" && selectedVial){
            Detalles.transform.GetChild(1).gameObject.SetActive(true);
            for (int i = 1; i <= Metales.transform.GetChild(0).childCount; i++) {
                Image img = Metales.transform.GetChild(0).GetChild(i - 1).GetComponent<Image>();

                if (img != null) {
                    int indiceSeleccionado = selectedVial ? selectedRawMetalVialIndex : selectedMetalVialIndex;

                    if (i - 1 == indiceSeleccionado){
                        if(i!=17){
                            Detalles.transform.GetChild(0).GetChild(((i-1) % 2)).GetChild((i-1) / 2).GetComponent<TextMeshProUGUI>().color = Color.yellow;
                            Detalles.transform.GetChild(1).GetChild(((i-1) / 4)+1).GetChild((i-1) % 4).GetComponent<TextMeshProUGUI>().color = Color.yellow;
                        }else{
                            Detalles.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.yellow;
                            Detalles.transform.GetChild(1).GetChild(5).GetComponent<TextMeshProUGUI>().color = Color.yellow;
                        }
                        img.color = Color.red;

                        int[] cantidades = new int[18];

                        foreach (RawMetal rm in pd.GetRawMetals()) {
                            cantidades[(int)rm.metal] = rm.amount;
                        }

                        string[] nombresIzquierda = {"Hierro","Estaño","Zinc","Cobre","Cadmio","Oro","Cromo","Aluminio"};
                        string[] nombresDerecha   = {"Acero","Peltre","Latón","Bronce","Bendaleo","Electro","Nicrosil","Duralumín"};

                        for (int j = 0; j < 8; j++) {
                            int metalIndex = 1 + j * 2;
                            string nombre = nombresIzquierda[j];
                            int cantidad = cantidades[metalIndex];
                            Detalles.transform.GetChild(0).GetChild(0).GetChild(j)
                                .GetComponent<TextMeshProUGUI>().text = $"{nombre}: {(cantidad > 0 ? (cantidad - rawMetalsUsed[metalIndex-1]) : 0)}";
                        }

                        for (int j = 0; j < 8; j++) {
                            int metalIndex = 2 + j * 2;
                            string nombre = nombresDerecha[j];
                            int cantidad = cantidades[metalIndex];
                            Detalles.transform.GetChild(0).GetChild(1).GetChild(j)
                                .GetComponent<TextMeshProUGUI>().text = $"{nombre}: {(cantidad > 0 ? (cantidad - rawMetalsUsed[metalIndex-1]) : 0)}";
                        }

                        int cantidadAtium = cantidades[17];
                        Detalles.transform.GetChild(0).GetChild(2)
                            .GetComponent<TextMeshProUGUI>().text = $"Atium: {(cantidadAtium > 0 ? (cantidadAtium - rawMetalsUsed[16]) : 0)}";

                    } else {
                        img.color = Color.white;
                        if(i!=17){
                            Detalles.transform.GetChild(0).GetChild(((i-1) % 2)).GetChild((i-1) / 2).GetComponent<TextMeshProUGUI>().color = Color.white;
                            Detalles.transform.GetChild(1).GetChild(((i-1) / 4)+1).GetChild((i-1) % 4).GetComponent<TextMeshProUGUI>().color = Color.white;
                        }else{
                            Detalles.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().color = Color.white;
                            Detalles.transform.GetChild(1).GetChild(5).GetComponent<TextMeshProUGUI>().color = Color.white;
                        }
                    }
                }
            }
            return;
        }

        if (grupoMetal == null) return;
        if (!inMetalPage) return;

        for (int i = 1; i <= grupoMetal.transform.childCount; i++){
            GameObject metal = grupoMetal.transform.GetChild(i-1).gameObject;
            UnityEngine.UI.Image img = metal.GetComponent<UnityEngine.UI.Image>();

            bool slotOcupado = 
                inWhatMetalPage[0] ? pd.IsAloMetalEquipped(i) :
                inWhatMetalPage[1] ? pd.IsFeruMetalEquipped(i) :
                inWhatMetalPage[2] ? pd.IsHemaMetalEquipped(i) : false;


            if (img != null){
                img.color = (i-1 == selectedIndex) ?
                    (slotOcupado ?
                        Color.red :
                        Color.magenta
                    ) :
                    (slotOcupado ?
                        Color.yellow : 
                        Color.white
                    );
            }else{
                Debug.LogWarning($"El metal {metal.name} no tiene Image asignado.");
            }
        }

        if (inWhatMetalPage[0]){
            if (txtAccion != null && txtPosicion != null && txtTipo != null){
                txtNombreAlo.text = nombreMetalAlo[selectedIndex];
                txtDescripcionAlo.text = descripcionMetalAlo[selectedIndex];

                txtAccion.text = selectedIndex%2 == 0 ? "Tirón" : "Empuje";

                txtPosicion.text = (selectedIndex % 4 == 1 || selectedIndex % 4 == 0) ? "Externo" : "Interno";

                txtTipo.text =
                (selectedIndex % 16 == 0 || selectedIndex % 16 == 1 || selectedIndex % 16 == 2 || selectedIndex % 16 == 3) ? "Físico" :
                (selectedIndex % 16 == 4 || selectedIndex % 16 == 5 || selectedIndex % 16 == 6 || selectedIndex % 16 == 7) ? "Mental" :
                (selectedIndex % 16 == 8 || selectedIndex % 16 == 9 || selectedIndex % 16 == 10 || selectedIndex % 16 == 11) ? "Temporal" :
                "Espiritual";
            }
        }else if (inWhatMetalPage[1]){
            if (txtNombreFeruquimia != null && txtDescripcionFeruquimia != null){
                txtNombreFeruquimia.text = nombreMetalFeru[selectedIndex];
                txtDescripcionFeruquimia.text = descripcionMetalFeru[selectedIndex];
            }
        }else if(inWhatMetalPage[2]){
            if (txtNombreHemalurgia != null && txtDescripcionHemalurgia != null){
                txtNombreHemalurgia.text = nombreMetalHema[selectedIndex];
                txtDescripcionHemalurgia.text = descripcionMetalHema[selectedIndex];
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