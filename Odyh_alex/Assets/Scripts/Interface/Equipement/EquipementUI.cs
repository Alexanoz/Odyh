﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipementUI : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private EquipementButton head, chest, feet, hand, onehand, bow, staff;
    
    public EquipementButton EquipementButton { get; set; }

    private static EquipementUI _equipementUi;

    public static EquipementUI EquipementUi
    {
        get
        {
            if (_equipementUi == null)
            {
                _equipementUi = FindObjectOfType<EquipementUI>();
            }

            return _equipementUi;
        }
    }

    private Player _player;

    public void OpenClose()
    {
        if (_canvasGroup.alpha == 1)
        {
            CloseButton();
        }
        else
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
        }
    }
    
    public void CloseButton()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }


    public void Equip(Equipement equipement)
    {
        switch (equipement.EquipementType)
        {
            case EquipementType.Casque:
                head.EquipEquipement(equipement);
                head.UpdateStats(equipement);
                break;
            case EquipementType.Torse:
                chest.EquipEquipement(equipement);
                chest.UpdateStats(equipement);
                break;
            case EquipementType.Bottes:
                feet.EquipEquipement(equipement);
                feet.UpdateStats(equipement);
                break;
            case EquipementType.Gants:
                hand.EquipEquipement(equipement);
                hand.UpdateStats(equipement);
                break;
            case EquipementType.Epee:
                onehand.EquipEquipement(equipement);
                onehand.UpdateStats(equipement);
                break;
            case EquipementType.Arc:
                bow.EquipEquipement(equipement);
                _player.BowAttackPossible = true;
                break;
            case EquipementType.Baton:
                staff.EquipEquipement(equipement);
                _player.MagicAttackPossible = true;
                break;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            OpenClose();
        }
    }
}
