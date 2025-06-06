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
    string[] descriptions = {
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
            metalDescription.text = descriptions[(int)unlockeableMetal - 1];
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
