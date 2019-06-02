﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler, Cliquable, IPointerEnterHandler, IPointerExitHandler
{
    
    //ref du sprite
    [SerializeField]
    private Image itemsprite;

    //stack servant a empiler/dépiler plusieurs objets identiques 
    public Stack<Item> itemStack = new Stack<Item>();

    public int Index { get; set; }
    
    
    //text fils de slot du nombre d'objets empilés
    [SerializeField]
    private Text stacktext;

    //bool pour savoir si le stack d'item est vide
    public bool Empty
    {
        get { return itemStack.Count == 0; }
    }
       
    //reference au script du sac dans lequel est le slot
    public Bag SlotBagScr { get; set; }
    
    
    
    //bool pour savoir si le stack est plein
    public bool Full
    {
        get
        {
            if (Empty || Itemscount < TheItem.Stacksize)
            {
                return false;
            }

            return true;
        }
        
    }

    //getter de l'item au dessus de la pile
    public Item TheItem
    {
        get
        {
            if (!Empty)
            {
                return itemStack.Peek();
                
            }

            return null;
        }
    }
    
    
    //ajout et suppression d'un item 
    public bool AddItem(Item item)
    {
        itemStack.Push(item);
        itemsprite.sprite = item.TheSprite;
        itemsprite.color = Color.white;
        item.Slot = this;
        StackSlotManage(this);
        return true;
    }

    public void Delete_Item(Item item)
    {
        if (!Empty)
        {
            itemStack.Pop();
            StackSlotManage(this);
        }
    }

    //Clear le slot (par exemple lorsque on jette un objet)
    public void Clear_slot()
    {
        if (Itemscount > 0)
        {
            itemStack.Clear();
        }
    }
    
    public bool StackItem(Item item)
    {
        if (!Empty && item.name == TheItem.name && itemStack.Count < item.Stacksize)
        {
            itemStack.Push(item);
            item.Slot = this;
            return true;
        }

        return false;
    }
    


    //interface IPointerClickHandler avec le clic droit puis clic gauche pour déplacement 
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && MoveManager.TheMoveManager.Itembougeable == null)
        {
            Using_Item();
        }
        
        
       if (eventData.button == PointerEventData.InputButton.Left)
        {
            
            if (Inventory.InventoryScr.TheSlot == null && !Empty)    //on instancie le slot s'il est null
            {
                if (MoveManager.TheMoveManager.Itembougeable != null)
                {
                    if (MoveManager.TheMoveManager.Itembougeable is BagItem)
                    {
                        if (TheItem is BagItem)
                        {
                            Inventory.InventoryScr.SwapBags(MoveManager.TheMoveManager.Itembougeable as BagItem, TheItem as BagItem);    
                        }
                    }
                    else if (MoveManager.TheMoveManager.Itembougeable is Equipement)
                    {
                        if (TheItem is Equipement && (TheItem as Equipement).EquipementType == (MoveManager.TheMoveManager.Itembougeable as Equipement).EquipementType) 
                        {
                            (TheItem as Equipement).Equip();
                            MoveManager.TheMoveManager.Drop();
                        }
                    }
                }
                else
                {
                    MoveManager.TheMoveManager.PickBougeable(TheItem );
                    Inventory.InventoryScr.TheSlot = this;
                }                
            }
            else if (Inventory.InventoryScr.TheSlot == null && Empty) //si on a un sac dans la main et qu'on le place dans notre inventaire
            {
                if (MoveManager.TheMoveManager.Itembougeable is BagItem)
                {
                    BagItem bag = (BagItem) MoveManager.TheMoveManager.Itembougeable;
                
                    if (bag.BagScr != SlotBagScr && Inventory.InventoryScr.EmptySlotNb - bag.Slotnumber > 0) //on check si on ne met pas le bag dans lui-même et qu'il y a assez de slots libre pour les items
                    {
                        AddItem(bag);
                        bag.BagButton.Delete_bag();
                        MoveManager.TheMoveManager.Drop();
                    }
                }
                else if (MoveManager.TheMoveManager.Itembougeable is Equipement)
                {
                    Equipement e = (Equipement) MoveManager.TheMoveManager.Itembougeable;
                    AddItem(e);
                    EquipementUI.EquipementUi.EquipementButton.Desequip(e); 
                    MoveManager.TheMoveManager.Drop();
                }
               
            }
            else if (Inventory.InventoryScr.TheSlot != null) //si le slot n'est pas null, on check les fonctions qui replace, switch ou bouge les items dans le sac
            {
                if (PutBack())
                {
                    MoveManager.TheMoveManager.Drop();
                    Inventory.InventoryScr.TheSlot = null;              
                }
                else if (Swap(Inventory.InventoryScr.TheSlot))
                {
                    MoveManager.TheMoveManager.Drop();
                    Inventory.InventoryScr.TheSlot = null;
                }
                else if (AddItems(Inventory.InventoryScr.TheSlot.itemStack))
                {
                    MoveManager.TheMoveManager.Drop();
                    //Inventory.InventoryScr.TheSlot.Image.color = new Color(0,0,0,0);
                    Inventory.InventoryScr.TheSlot = null;
                }
                
            }
            
        }  
    }


    //Permet de savoir si on veux replacer l'item a son endroit initial
    private bool PutBack()
    {
        if (Inventory.InventoryScr.TheSlot == this)
        {
            Inventory.InventoryScr.TheSlot.Image.color = Color.white;
            return true;
        }

        return false;
    }
    
    //Fonction de swap entre deux slots A et B
    private bool Swap(Slot slotscr)
    {
        if (Empty)
        {
            return false;
        }

        if (slotscr.TheItem.GetType() != TheItem.GetType() || slotscr.Itemscount + Itemscount > TheItem.Stacksize)
        {
            Stack<Item> newstack = new Stack<Item>(slotscr.itemStack); //Copie des items de A
            slotscr.itemStack.Clear();                                 //clear A
            slotscr.AddItems(itemStack);                               //copie les éléments de B dans A
            itemStack.Clear();                                         //clear B
            AddItems(newstack);                                        //élément de la copie de A dans B

            return true;
        }

        return false;
    }
    
    //Bool pour savoir si on veux placer l'item dans un autre slot vide ou équipé d'objet du même type
    public bool AddItems(Stack<Item> stack)
    {
        if (Empty || stack.Peek().GetType() == TheItem.GetType())
        {
            int count = stack.Count;

            for (int i = 0; i < count; i++)   //on essaye d'empiler tous les item de la stack d'origine dans le slot
            {
                if (Full)
                {
                    return false;
                }

                AddItem(stack.Pop());
            }

            return true;
        }

        return false;
    }
   
    
    //on check si l'item est bien Utilisable avant de le Use()
    public void Using_Item()
    {
        if (TheItem is Utilisable)
        {
            (TheItem as Utilisable).Use();
            
        }
        else if (TheItem is Equipement)
        {
            (TheItem as Equipement).Equip();     
        }
    }
    
    
    //fonction updatant l'interface des slots (ici implémenté avec l'interface cliquable)
    public void StackSlotManage(Cliquable cliquable)
    {
        //si on a plus d'un item empilé on instancie le texte associé au nombre d'item sinon on enleve le texte (en mettant sa couleur a 0)
        if (cliquable.Itemscount > 1)
        {
            cliquable.StackText.text = cliquable.Itemscount.ToString();
            cliquable.StackText.color = Color.white;
            cliquable.Image.color = Color.white;
        }
        else
        {
            cliquable.StackText.color = new Color(0, 0, 0, 0);
        }
        
        
        if (cliquable.Itemscount == 0)    //enleve l'icone si il n'y a plus d'objet
        {
            cliquable.Image.color = new Color(0, 0, 0, 0);
            cliquable.StackText.color = new Color(0, 0, 0, 0);
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StackSlotManage(this);
    }

    
    //implémentation de l'interface Cliquable
    public Image Image
    {
        get { return itemsprite; }
        set { itemsprite = value; }
    }

    public int Itemscount
    {
        get { return itemStack.Count; }
    }

    public Text StackText
    {
        get { return stacktext; }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!Empty)
        {
            UI.UserInterface.ShowInfos(transform.position, TheItem);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UI.UserInterface.HideInfos();
    }
}
