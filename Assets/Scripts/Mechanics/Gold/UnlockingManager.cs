using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnlockingManager : MonoBehaviour{

    public Metal unlockeableMetal;
    public MetalArt unlockeableMetalArt;

    public VialType unlockableVialType;
    public int vialId;

    public Sprite[] aloMetalSymbols;
    public Sprite[] feruMetalSymbols;
    public Sprite[] hemaMetalSymbols;
    public Sprite[] vialSprites;
    string[] names = {
        "HIERRO",
        "ACERO",
        "ESTAÑO",
        "PELTRE",
        "ZINC",
        "LATÓN",
        "COBRE",
        "BRONCE",
        "CADMIO",
        "BENDALEO",
        "ORO",
        "ELECTRO",
        "CROMO",
        "NICROSIL",
        "ALUMINIO",
        "DURALUMINIO",
        "ATIUM"
    };
    string[] descriptionsAlo = {
        // HIERRO
        "Al quemar el metal, manten pulsado Q para con A y D elegir una línea alomántica, \nmientras tengas pulsado Q, al pulsar U tirarás del objeto.\n"+
        "Si el objeto es más ligero, saldrá lanzado hacia ti; si es más pesado, tú saldrás disparado hacia él.",

        // ACERO
        "Al quemar el metal, manten pulsado Q para con A y D elegir una línea alomántica, \nmientras tengas pulsado Q, al pulsar I empujarás del objeto.\n"+
        "Si el objeto es más ligero, saldrá despedido; si es más pesado, tú saldrás despedido.",

        // ESTAÑO
        "Al quemar el metal, tus sentidos se agudizarán, permitiéndote ver hasta en las nieblas más densas\n"+
        "Este metal es un arma de doble filo, pues cualquier ruido o luz fuerte puede confundirte.",

        // PELTRE
        "Al quemar el metal, tu cuerpo se volverá más fuerte y resistente, tus golpes harán más daño mientras que los recibidos harán menos\n"+
        "Este metal es un arma de doble filo, pues si las heridas son letales y se deja de quemar, sucumbirás a las heridas.",

        // ZINC
        "Al quemar el metal, manten pulsado Q para con A y D elegir un objetivo válido, \nmientras tengas pulsado Q, al pulsar O enajenarás al objetivo.\n"+
        "Su daño se verá incrementado, pero también su furia, atacando al resto de enemigos que tenga a su alrededor.",

        // LATÓN
        "Al quemar el metal, manten pulsado Q para con A y D elegir un objetivo válido, \nmientras tengas pulsado Q, al pulsar P apaciguarás al objetivo.\n"+
        "Su daño se verá reducido al igual que su velocidad, llegando a ignorarte por completo si sus emociones ya estaban de por sí apaciguadas.",

        // COBRE
        "Al quemar el metal, toda la alomancia usada se ocultará a los sentidos del bronce creando una nube de cobre, haciendo que sea más difícil detectarte.\n"+
        "La alomancia mental de zinc o latón no funcionará si estás quemando cobre.",

        // BRONCE
        "Al quemar el metal, toda la alomancia de alrededor será mostrada en forma de pulsos, aunque estos pueden ser tapados por los brumosos del cobre.\n"+
        "Dependiendo de la velocidad, punto de inicio y color, se puede saber qué metal está quemando.\n"+
        "Mezclado con duraluminio, se pueden atravesar las nubes de cobre.",

        // CADMIO
        "Al quemar el metal, se genera una burbuja de velocidad en el punto donde lo quemaste haciendo que el exterior se vea acelerado en el tiempo.\n"+
        "Hay plantas espinosas que al no sentir movimiento durante largos periodos de tiempo, guardan las espinas.\n"+
        "Cuando se usa con duraluminio, la feruquimia se ve afectada.\n",

        // BENDALEO
        "Al quemar el metal, se genera una burbuja de velocidad en el punto donde lo quemaste haciendo que el exterior se vea ralentizado en el tiempo.\n"+
        "Cuando se usa con duraluminio, el tiempo se contrae a tal punto que la burbuja y el paso del tiempo desaparecen.",

        // ORO
        "Si de verdad estas viendo este texto en el juego, algo he hecho mal programándolo.\n"+
        "En principio nada debería desbloquear el oro ya que es el primer metal.",

        // ELECTRO
        "Al quemar el metal, proyectas múltiples sombras de electro, mostrándote todos los caminos que puedes tomar en el futuro muy próximo.\n"+
        "Su utilidad deriva al usarlo contra un brumoso de atium, ya que tu futuro estará tan indeterminado que no se podrá conocer.\n",

        // CROMO
        "Al quemar el metal, y atacar a un brumoso, agotarás sus reservas alománticas temporalmente.\n"+
        "Este metal se consume rápido con cada ataque, pero en momentos específicos puede ayudar mucho.\n",

        // NICROSIL
        "Al quemar el metal, y atacar a un brumoso, le infundirás un destello de poder, pudiendo llegar a confundirlos durante un periodo de tiempo y agotándole las reservas.\n"+
        "Este metal se consume rápido con cada ataque, pero en momentos específicos puede ayudar mucho.\n",

        // ALUMINIO
        "Al quemar el metal, las reservas de todos los metales se consumirán a una gran velocidad, despojando de la alomancia al que lo queme.\n"+
        "Su uso no es muy recomendable, pero nunca se sabe a ciencia cierta.\n",

        // DURALUMINIO
        "Al quemar el metal, las reservas de todos los metales que se estén quemando se quemarán mucho más rápido, permitiendo aumentar la potencia de estos en grandes cantidades.\n"+
        "Cada metal tiene su potencial mejorado al usarlo con duraluminio, algunos hasta adquieren efectos adicionales.\n",

        // ATIUM
        "Al quemar el metal, todo enemigo mostrará qué hará en el futuro próximo, otorgándote la capacidad de esquivar fácilmente, pero aún te pueden dar.\n"+
        "Un metal divino que crece en geodas muy frágiles difíciles de encontrar, de ahí su escasez.\n",
    };
    private string[] descriptionsFeru = new string[] {
        // Brazal de Hierro
        "Al guardar peso en hierro, el peso disminuirá permitiendo saltar más alto;\n"+
        "Al decantar peso del hierro, el peso aumentará acortando el salto",

        // Bracil de Acero
        "Al guardar velocidad en acero, la velocidad de movimiento se verá reducida;\n" +
        "Al decantar velocidad del acero, la velocidad de movimiento aumentará considerablemente",

        // Esclava de Estaño
        "Al guardar percepción en estaño, la percepción del entorno será peor;\n" +
        "Al decantar percepción del estaño, el entorno aparenta esclarecerse.",

        // Ajorca de Peltre
        "Al guardar fuerza en peltre, los golpes harán menos daño;\n" +
        "Al decantar fuerza del peltre, los golpes serán empoderados golpeando más fuerte.",

        // Pulsera de Zinc
        "Al guardar velocidad de pensamiento en zinc, al ser golpeado habrá menos tiempo para recuperarse;\n" +
        "Al decantar velocidad de pensamiento del zinc, la ventana para volver a ser golpeado es más amplia.",

        // Bracil de Latón
        "Al guardar calor en latón, el daño entre golpe y golpe del fuego es mayor;\n" +
        "Al decantar calor del latón, el daño del fuego se repite con más frecuencia.",

        // Brazal de Cobre
        "Al guardar recuerdos en cobre, estos se olvidarán y pasarán a guardarse en el cobre, inmutables.\n" +
        "Al decantar recuerdos del cobre, se tendrá acceso a los recuerdos tal y como se guardaron.",

        // Pulsera de Bronce
        "Al guardar vigilia en bronce, la duración del bloqueo es menor;\n" +
        "Al decantar vigilia del bronce, el bloqueo protege durante más tiempo.",

        // Brazalete de Cadmio
        "Al guardar aliento en cadmio, el tiempo de aguante en las brumas tóxicas es menor;\n" +
        "Al decantar aliento del cadmio, las brumas tóxicas tardarán más en sofocar.",

        // Bracil de Bendaleo
        "Al guardar nutrición en bendaleo, el poder curativo de los viales es menor;\n" +
        "Al decantar nutrición el bendaleo, al consumir un vial curará más que de costumbre.",

        // Ajorca de Oro
        "Al guardar salud en oro, la vida máxima será reducida a un tercio;\n" +
        "Al decantar salud del oro, la vida se rellenará a una velocidad considerable.",

        // Brazalete de Electro
        "Al guardar determinación en electro, cualquier golpe puede ser crítico.\n" +
        "Al decantar determinación del electro, el daño recibido será reducido.",

        // Brazal de Cromo
        "Al guardar fortuna en cromo, el botín encontrado por el mundo será menor;\n" +
        "Al decantar fortuna del cromo, el botín encontrado será mayor.",

        // Esclava de Nicrosil
        "Al guardar investidura en nicrosil, la alomancia tendrá que consumir más metales de las reservas;\n" +
        "Al decantar investidura del nicrosil, la alomancia se vuelve más eficiente, requiriendo menos metales.",

        // Brazalete de Aluminio
        "Al guardar identidad en aluminio, la locura aumenta con mayor velocidad;\n" +
        "Al decantar identidad del aluminio, el ciclo de locura se revierte a un ritmo lento.",

        // Pulsera de Duralumín
        "Al guardar conexión en duralumín, los precios en las tiendas se ven aumentados;\n" +
        "Al decantar conexión del duralumín, las tiendas confían más, rebajando los precios.",

        // Esclava de Atium
        "Al guardar edad en atium, se recibe daño poco a poco, pudiendo ser letal;\n" +
        "Al decantar edad del atium, cualquier daño que sea letal será inútil."
    };
    private string[] descriptionsHema = new string[] {
        //Clavo de Hierro
        "Extraído del pie de un padre skaa crucificado buscando a su hijo.\n"+
        "Impregnado de su fortaleza, otorga resistencia a los empujes.",

        //Tornillo de Acero
        "Arrebatado del cráneo de un prestigioso ladrón.\n"+
        "Imbuido con su presteza, otorga velocidad de movimiento.",

        //Tachuela de Estaño
        "Quitada del costado de una recolectora de objetos valiosos.\n"+
        "Pringada de su agudeza, permite ver mejor.",

        //Punta de Peltre
        "Sacada de la cuenca de un violento más.\n"+
        "Empapada de su volatilidad, potencia los ataques.",

        //Tachuela de Zinc
        "Extirpada con tristeza del cuello de un niño perdido por las calles.\n"+
        "Bañada en su agudeza, aumenta el tiempo de invulnerabilidad tras un golpe",

        //Escarpia de Latón
        "Arrancada de la sien de un pirómano empedernido.\n"+
        "Manchada por su amor al fuego, reduce el daño de este.",

        //Clavo de Cobre
        "Retirado con respeto del pecho de un remunerado cartógrafo.\n"+
        "Haciendo acopio de su saber, muestra en el mapa los puntos de interés.",

        //Alcayata de Bronce
        "Arrebatada de la frente de un matasanos desquiciado.\n"+
        "Embebida de su conocimiento, muestra la salud de los enemigos.",

        //Tornillo de Cadmio
        "Sacado del pulmón de un minero en las cuevas de carbón.\n"+
        "Imbuido de su capacidad, aumenta el tiempo que se puede aguantar la respiración.",

        //Punta de Bendaleo
        "Quitada del hombro de un pescador mecido por las mareas.\n"+
        "Empapada de su sino, aumenta el poder curativo de los viales de salud.",

        //Tachuela de Oro
        "Extraída del antebrazo de un aclamado doctor sin título.\n"+
        "Bañada en su saber, otorga una curación lenta pero sin pausa.",

        //Alcayata de Electro
        "Extirpada del brazo de un guardaespaldas violento sin familia.\n"+
        "Pringada de su agilidad, permite evitar el daño de algunos golpes.",

        //Escarpia de Cromo
        "Retirada de la mano un ilusionista de un noble.\n"+
        "Imbuida con su naturaleza embaucadora, aumenta la cantidad de recursos generados.",

        //Clavo de Nicrosil
        "Sacado del omóplato de un vidente sin oficio ni beneficio.\n"+
        "Embebido de su investidura, aumenta la eficacia de la alomancia.",

        //Tornillo de Aluminio
        "Quitado del abdomen de un embalsamador altruista.\n"+
        "Anula por completo la alomancia y la feruquimia.",

        //Alcayata de Duralumín
        "Arrancada con rabia de la espalda de un Koloss.\n"+
        "Manchada de su inhumanidad, mejora los efectos hemalúrgicos",

        //Escarpia de Atium
        "Arrancada con saña de la pierna de un Inquisidor.\n"+
        "Manchada de su potencia, otorga la probabilidad de esquivar cualquier ataque.",
    };

    public GameObject panel;
    public TextMeshProUGUI title;
    public TextMeshProUGUI metalName;
    public TextMeshProUGUI metalDescription;
    public Image[] metalSymbols;

    Image panelImage;

    bool playerInside = false;
    bool unlocked = false;
    
    PlayerScript playerScript;
    PlayerData pd;

    Color color = new Color(1f,1f,1f,0f);
    public bool showingPanel = false;
    public bool canHidePanel = false;

    Animator animator;

    void Start(){
        playerScript = FindObjectOfType<PlayerScript>();
        animator = GetComponent<Animator>();

        if (panel == null) {
            return;
        }

        if(unlockeableMetalArt == MetalArt.ALLOMANCY){
            unlocked = playerScript.pd.IsAloMetalUnlocked((int)unlockeableMetal);

        }else if(unlockeableMetalArt == MetalArt.FERUCHEMY){
            unlocked = playerScript.pd.IsFeruMetalUnlocked((int)unlockeableMetal);

        }else if(unlockeableMetalArt == MetalArt.HEMALURGY){
            unlocked = playerScript.pd.IsHemaMetalUnlocked((int)unlockeableMetal);

        }else if(unlockeableMetalArt == MetalArt.NULL){
            if(unlockableVialType == VialType.HEALTH){
                unlocked = playerScript.pd.IsHealthVialUnlocked(vialId);
            }else if(unlockableVialType == VialType.METAL){
                unlocked = playerScript.pd.IsMetalVialUnlocked(vialId);
            }else if(unlockableVialType == VialType.MAX_HEALTH){
                unlocked = playerScript.pd.IsMaxHealthVialUnlocked(vialId);
            }
        }
        animator.SetBool("Retrieved", unlocked);
        panelImage = panel.GetComponent<Image>();
        SetAlpha(metalName, 0f);
        SetAlpha(metalDescription, 0f);
        foreach (Image img in metalSymbols) {
            SetAlpha(img, 0f);
        }
        panel.SetActive(false);
    }

    void Update() {
        if(unlocked) return;
        pd = playerScript.pd;

        AloMetal amGol = pd.GetAloMetalIfEquipped((int)Metal.GOLD);

        if (playerScript.interacting && playerInside && !showingPanel && ((unlockeableMetalArt == MetalArt.FERUCHEMY || unlockeableMetalArt == MetalArt.HEMALURGY || unlockeableMetalArt == MetalArt.NULL) ? true : (amGol != null && amGol.IsBurning()))) {
            showingPanel = true;
            UnlockMetalToPD();
        }
    }

    void UnlockMetalToPD() {
        animator.SetBool("Retrieved", true);
        unlocked = true;

        if(unlockeableMetalArt == MetalArt.ALLOMANCY){
            pd.UnlockAloMetal((int)unlockeableMetal);
        }else if(unlockeableMetalArt == MetalArt.FERUCHEMY){
            pd.UnlockFeruMetal((int)unlockeableMetal);
        }else if(unlockeableMetalArt == MetalArt.HEMALURGY){
            pd.UnlockHemaMetal((int)unlockeableMetal);
        }else if(unlockeableMetalArt == MetalArt.NULL){

            if(unlockableVialType == VialType.HEALTH){
                pd.UnlockHealthVial((int)vialId);
            }else if(unlockableVialType == VialType.METAL){
                pd.UnlockMetalVial((int)vialId);
            }else if(unlockableVialType == VialType.MAX_HEALTH){
                pd.UnlockMaxHealthVial((int)vialId);
            }
        }

        panel.SetActive(true);
        if(unlockeableMetalArt != MetalArt.NULL){
            string tipo = unlockeableMetalArt == MetalArt.ALLOMANCY ? "ALOMÁNTICO" :
                        unlockeableMetalArt == MetalArt.FERUCHEMY ? "FERUQUÍMICO" :
                        unlockeableMetalArt == MetalArt.HEMALURGY ? "HEMALÚRGICO" : 
                        "¿?¿?¿?";

            title.text = $"METAL {tipo} DESBLOQUEADO";

            metalName.text = names[(int)unlockeableMetal - 1];

            metalDescription.text = unlockeableMetalArt == MetalArt.ALLOMANCY ? descriptionsAlo[(int)unlockeableMetal - 1] :
                                    unlockeableMetalArt == MetalArt.FERUCHEMY ? descriptionsFeru[(int)unlockeableMetal - 1] :
                                    unlockeableMetalArt == MetalArt.HEMALURGY ? descriptionsHema[(int)unlockeableMetal - 1] :
            "¿?¿?¿?";

            foreach(Image i in metalSymbols){
                if(unlockeableMetalArt == MetalArt.ALLOMANCY){
                    i.sprite = aloMetalSymbols[(int)unlockeableMetal - 1];

                }else if(unlockeableMetalArt == MetalArt.FERUCHEMY){
                    i.sprite = feruMetalSymbols[(int)unlockeableMetal - 1];

                }else if(unlockeableMetalArt == MetalArt.HEMALURGY){
                    i.sprite = hemaMetalSymbols[(int)unlockeableMetal - 1];

                }
            }
        }else{
            title.text = $"VIAL {(unlockableVialType == VialType.HEALTH ? "DE VIDA" : (unlockableVialType == VialType.METAL ? "METÁLICO" : "DE VIDA MÁXIMA") )} DESBLOQUEADO";
            metalName.text = "";
            metalDescription.text = unlockableVialType == VialType.HEALTH ? @"Vial cuyo contenido es desconocido, pero que al ser ingerido cura heridas, golpes tanto las leves como las más mortales de ellas." : 
                                    unlockableVialType == VialType.METAL ? @"Lleno de alcohol para evitar la oxidación, tan sólo le hace falta metal en polvo y un alomante será imparable." :
                                                                            @"Ni el sabor ni la textura son de agrado de nadie, mas nadie puede negar la resistencia que otorga al ingerirse.";
            foreach(Image i in metalSymbols){
                if(unlockableVialType == VialType.HEALTH){
                    i.sprite = vialSprites[0];

                }else if(unlockableVialType == VialType.METAL){
                    i.sprite = vialSprites[1];

                }else if(unlockableVialType == VialType.MAX_HEALTH){
                    i.sprite = vialSprites[2];

                }
            }
        }

        if (panelImage != null) {
            Color c = panelImage.color;
            panelImage.color = new Color(c.r, c.g, c.b, 0f);
            StartCoroutine(ShowPanel());
        }
    }

    IEnumerator ShowPanel() {
        Time.timeScale = 0f;
        float alpha = 0f;
        while (alpha < 1f) {
            alpha = Mathf.Min(1f, alpha + 0.05f);

            SetAlpha(panelImage, alpha);
            SetAlpha(title, alpha);
            SetAlpha(metalName, alpha);
            SetAlpha(metalDescription, alpha);
            foreach (Image img in metalSymbols) {
                SetAlpha(img, alpha);
            }
            yield return new WaitForSecondsRealtime(.05f);
        }
        StartCoroutine(WaitPanel());
    }

    IEnumerator WaitPanel() {
        yield return new WaitForSecondsRealtime(2f);
        canHidePanel = true;
        yield return new WaitForSecondsRealtime(5f);
        playerScript.hideUnlockingPanel = false;
        StartCoroutine(HidePanel());
    }

    IEnumerator HidePanel() {
        float alpha = 1f;
        while (alpha > 0f) {
            alpha = Mathf.Max(0f, alpha - 0.05f);

            SetAlpha(panelImage, alpha);
            SetAlpha(title, alpha);
            SetAlpha(metalName, alpha);
            SetAlpha(metalDescription, alpha);
            foreach (Image img in metalSymbols) {
                SetAlpha(img, alpha);
            }
            yield return new WaitForSecondsRealtime(.05f);
        }
        Time.timeScale = 1f;
        panel.SetActive(false);
        showingPanel = false;
    }

    void SetAlpha(Graphic graphic, float alpha) {
        Color c = graphic.color;
        graphic.color = new Color(c.r, c.g, c.b, alpha);
    }


    void OnTriggerStay2D(Collider2D other){
        if (other.tag == "Player"){
            playerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if (other.tag == "Player"){
            playerInside = false;
        }
    }
}
