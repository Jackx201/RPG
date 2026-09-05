 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using UnityEngine.UI;
 public class TreasureChest : Interactable
 {
     [Header("Contents")]
    public InventoryItem content;
    public bool isOpen;
    public BoolValue storedOpen;
    public Inventory playerInventory;
    [Header("Signals and Dialog")]
    public SignalSender context;
    public SignalSender raiseItem;
    public GameObject dialogBox;
    public Text dialogText;
    [Header("Animation")]
    private Animator anim;


    void Start()
    {
        anim = GetComponent<Animator>();
        isOpen = storedOpen.value;
        if(isOpen){
            anim.SetBool("opened", true);
        }
    }

   
    void Update()
    {
       
        if(Input.GetButtonDown("attack") && playerInRange)
        {
            if(!isOpen)
            {
                OpenChest();
            } else {
                OpenedChest();
            }
        }
    }

 public void OpenChest()
 {
     //Activar El Dialogo
     dialogBox.SetActive(true);
     //Añadir Texto
     dialogText.text = content.myDescription;
     //Agregar Objetos al Inventario
     playerInventory.AddItem(content);
     context.Raise();
     if (animm != null)
     {
         animm.SetAnimParameter("contextActive", false);
     }
     //Dejar Abierto el cofre
     isOpen = true;
     anim.SetBool("opened", true);
     storedOpen.value = isOpen;
 }

 public void OpenedChest()
 {
    
     dialogBox.SetActive(false);
     raiseItem.Raise();
    
 }

     private void OnTriggerEnter2D(Collider2D other)
     {
         if (other.CompareTag("Player") && !other.isTrigger && !isOpen){
             context.Raise();
             playerInRange = true;
             if (animm != null)
             {
                 animm.SetAnimParameter("contextActive", true);
             }
         }
     }

     private void OnTriggerExit2D(Collider2D other)
     {
         if (other.CompareTag("Player") && !other.isTrigger)
         {
             if (!isOpen)
             {
                 context.Raise();
             }
             playerInRange = false;
             if (animm != null)
             {
                 animm.SetAnimParameter("contextActive", false);
             }
         }
     }

 }

