using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

// Control the top item movement between gamegrid containers
// this script is attached to the base -> BaseObject/Object/TopObject
public class TopItemController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SpriteResolver spriteResolver;
    private StoreGameObject storeGameobject;
    private GameGridObject gameGridObject;
    private GameObject saveObjButton, acceptButton, cancelButton;

    public void Start()
    {
        storeGameobject = null;
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        spriteResolver = transform.GetComponent<SpriteResolver>();
        SetEditPanelButtonClickListeners();
        HideEditPanel();
    }

    private void SetEditPanelButtonClickListeners()
    {
        saveObjButton = transform.Find(Settings.ConstEditTopItemMenuPanel + "/" + Settings.ConstEditStoreMenuSave).gameObject;
        Button save = saveObjButton.GetComponent<Button>();
        save.onClick.AddListener(ButtonsClickListener);

        acceptButton = transform.transform.Find(Settings.ConstEditTopItemMenuPanel + "/" + Settings.ConstEditStoreMenuButtonAccept).gameObject;
        Button accept = acceptButton.GetComponent<Button>();
        accept.onClick.AddListener(ButtonsClickListener);

        cancelButton = transform.transform.Find(Settings.ConstEditTopItemMenuPanel + "/" + Settings.ConstEditStoreMenuButtonCancel).gameObject;
        Button cancel = cancelButton.GetComponent<Button>();
        cancel.onClick.AddListener(ButtonsClickListener);

        Util.IsNull(saveObjButton, "saveObjButton is null in TopItemController/SetEditPanelButtonClickListeners");
        Util.IsNull(acceptButton, "acceptButton is null in TopItemController/SetEditPanelButtonClickListeners");
        Util.IsNull(cancelButton, "cancelButton is null in TopItemController/SetEditPanelButtonClickListeners");
    }

    public void HideTopItem()
    {
        if (gameGridObject.Type == ObjectType.BASE_CONTAINER && storeGameobject != null)
        {
            spriteRenderer.color = new Color(0, 0, 0, 0);
        }
    }

    public void ShowTopItem()
    {
        if (gameGridObject.Type == ObjectType.BASE_CONTAINER && storeGameobject != null)
        {
            spriteRenderer.color = new Color(0, 0, 0, 1);
            spriteRenderer.color = Util.Available;
        }
    }

    public void SetTopItem(StoreGameObject obj)
    {
        if (gameGridObject.Type == ObjectType.BASE_CONTAINER)
        {
            spriteResolver.SetCategoryAndLabel(obj.SpriteLibCategory, obj.Identifier);
            ShowTopItem();
            ShowBuyEditPanel();
            storeGameobject = obj;
        }
        else
        {
            GameLog.LogWarning("You can only set an item on top of a container");
        }
    }

    public StoreGameObject GetTopItem()
    {
        return storeGameobject;
    }

    public void ButtonsClickListener()
    {
        Debug.Log("Click listener ");
    }

    public void ShowBuyEditPanel()
    {
        saveObjButton.SetActive(false);
        acceptButton.SetActive(true);
        cancelButton.SetActive(true);
    }

    public void HideEditPanel()
    {
        saveObjButton.SetActive(false);
        acceptButton.SetActive(false);
        cancelButton.SetActive(false);
    }

    public void SetGamegridObject(GameGridObject obj)
    {
        gameGridObject = obj;
    }
}