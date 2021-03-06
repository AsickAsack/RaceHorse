using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
{
    public enum SlotState
    {
        All,Weapon,Use,Etc
    }


    public InventorySlot[] Slot;
    public List<Item> items = new List<Item>();
    public SlotState slotState = SlotState.All;
    public InventorySlot CurItem;
    public GameObject[] ButtonSelectImage;
    public GameObject ItemDespanel;
    public Text[] ItemDesText;
    public GameObject QuickRegisterBtn;
    public GameObject[] QuickSlot;
    public Text GoldText;

    private void Awake()
    {
        Slot = this.transform.GetComponentsInChildren<InventorySlot>();
    }

    //인벤토리 열때
    public void OpenInventory()
    {
        ResetSlots();
        slotState = SlotState.All;
        ButtonSelectImage[0].SetActive(true);
        GoldText.text = GameData.Instance.playerdata.money.ToString("N0") + " 골드";

        for (int i=0;i<GameData.Instance.playerdata.myItems.Count;i++)
        {
            Slot[i].itemdata = GameData.Instance.playerdata.myItems[i];
            Slot[i].Icon.sprite = UIManager.Instance.ItemIcon[GameData.Instance.playerdata.myItems[i].itemData.ItemCode];
            Slot[i].Icon.gameObject.SetActive(true);
            if(GameData.Instance.playerdata.myItems[i].itemData.myType == ItemType.Equip)
            {
                if (GameData.Instance.playerdata.myItems[i].IsEquip)
                {
                    Slot[i].Equip.SetActive(true);
                }
            }
            else
            {
                Slot[i].ItemCountTx.text = GameData.Instance.playerdata.myItems[i].ItemCount.ToString("N0");
                Slot[i].ItemCountTx.gameObject.SetActive(true);
            }       
        }
    }


    //인벤토리 끌때
    public void ExitInvetory()
    {
        SoundManager.Instance.PlayEffect1Shot(10);

        if (CurItem != null)
        {
            CurItem.SelectImage.SetActive(false);
            CurItem.myAnim.SetBool("IsSelect", false);

            if (QuickRegisterBtn.activeSelf)
                QuickRegisterBtn.SetActive(false);
        }

    }



    //카테고리 바꿀때
    public void Changebutton(int index)
    {
        SoundManager.Instance.PlayEffect1Shot(10);

        for(int i=0;i<ButtonSelectImage.Length;i++)
        {
            ButtonSelectImage[i].SetActive(i == index);
        }

        switch(index)
        {

            case 0:
                ResetSlots();
                OpenInventory();
                break;

            case 1:
                slotState = SlotState.Weapon;
                ChangeSlot(ItemType.Equip);
                break;
            case 2:
                slotState = SlotState.Use;
                ChangeSlot(ItemType.Use);
                break;
            case 3:
                slotState = SlotState.Etc;
                ChangeSlot(ItemType.Etc);
                break;
        }

    }

    //슬롯 바꿀때
    public void ChangeSlot(ItemType itemType)
    {
        ResetSlots();
        if(items != null)
        items.Clear();
        QuickRegisterBtn.SetActive(false);

        items = GameData.Instance.playerdata.myItems.FindAll(x => x.itemData.myType == itemType);

        for (int i = 0; i < items.Count; i++)
        {
            Slot[i].itemdata = items[i];
            Slot[i].Icon.sprite = UIManager.Instance.ItemIcon[items[i].itemData.ItemCode];
            Slot[i].Icon.gameObject.SetActive(true);
            if (items[i].itemData.myType == ItemType.Equip)
            {
                if (items[i].IsEquip)
                {
                    Slot[i].Equip.SetActive(true);
                }
            }
            else
            {
                Slot[i].ItemCountTx.text = GameData.Instance.playerdata.myItems[i].ItemCount.ToString("N0");
                Slot[i].ItemCountTx.gameObject.SetActive(true);
            }
        }
    }

    //슬롯 리셋할때
    public void ResetSlots()
    {
        if (CurItem != null)
        {
            CurItem.myAnim.SetBool("IsSelect", false);
            CurItem.SelectImage.SetActive(false);
        }
        for (int i=0;i<Slot.Length;i++)
        {
            Slot[i].Icon.gameObject.SetActive(false);
            Slot[i].ItemCountTx.gameObject.SetActive(false);
            Slot[i].Equip.SetActive(false);
        }

    }


    //아이템을 눌렀을때 루틴
    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>() != null)
        {
            SoundManager.Instance.PlayEffect1Shot(10);

            QuickRegisterBtn.SetActive(false);
            if (CurItem != null)
            {
                CurItem.myAnim.SetBool("IsSelect", false);
                CurItem.SelectImage.SetActive(false);   
            }

            CurItem = eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>();

            if (CurItem.Icon.gameObject.activeSelf)
            {
                CurItem.myAnim.SetBool("IsSelect", true);
                CurItem.SelectImage.SetActive(true);
                ItemDespanel.transform.position = CurItem.transform.position+new Vector3(180.0f, 40.0f, 0.0f);
                ItemDespanel.gameObject.SetActive(true);
                ItemDesText[0].text = CurItem.itemdata.itemData.ItemName;
                ItemDesText[1].text = CurItem.itemdata.itemData.ItemDes;

                if(CurItem.itemdata.itemData.myType == ItemType.Use)
                {
                    QuickRegisterBtn.transform.position = CurItem.transform.position - new Vector3(100.0f, 0.0f, 0.0f);
                    QuickRegisterBtn.SetActive(true);
                }
            }
        }
      
    }


    //눌렀다 떼면 설명창 꺼줌
    public void OnPointerUp(PointerEventData eventData)
    {
        if (CurItem != null)
        {
            ItemDespanel.gameObject.SetActive(false);
        }
    }


    //퀵슬롯 등록
    public void QuickRegister(int index)
    {
        SoundManager.Instance.PlayEffect1Shot(10);
        QuickSlot[index].GetComponent<QuicSlot>().SetQuickSlot(CurItem.itemdata);
    }


    //아이템 쓸때
    public void InVenUseItem()
    {
        if (CurItem != null)
        {
            if (CurItem.itemdata.ItemCount > 0)
            {
                CurItem.itemdata.itemData.UseItem(CurItem.itemdata.itemData.ItemCode);
                CurItem.itemdata.ItemCount--;
                if (CurItem.itemdata.ItemCount == 0)
                {
                    GameData.Instance.playerdata.myItems.Remove(CurItem.itemdata);
                    QuickRegisterBtn.SetActive(false);
                }
                Changebutton((int)slotState);
                QuickSlot[0].GetComponent<QuicSlot>().RePrintQuickSlot();
                QuickSlot[1].GetComponent<QuicSlot>().RePrintQuickSlot();
            }
        }
    }

}
