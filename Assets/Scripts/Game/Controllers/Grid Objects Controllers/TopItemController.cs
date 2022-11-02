using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

// Control the top item movement between gamegrid containers
// this script is attached to the base -> BaseObject/Object/TopObject
public class TopItemController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SpriteResolver spriteResolver;
    private StoreGameObject obj;
    private GameObject saveObjButton, acceptButton, cancelButton;

    public void Start()
    {
        obj = null;
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        spriteResolver = transform.GetComponent<SpriteResolver>();
        SetEditPanelButtonClickListeners();
        HideEditPanel();
    }

    private void SetEditPanelButtonClickListeners()
    {
        saveObjButton = transform.Find(Settings.ConstEditStoreMenuSave).gameObject;
        Button save = saveObjButton.GetComponent<Button>();
        save.onClick.AddListener(ButtonsClickListener);

        acceptButton = transform.transform.Find(Settings.ConstEditStoreMenuButtonAccept).gameObject;
        Button accept = acceptButton.GetComponent<Button>();
        accept.onClick.AddListener(ButtonsClickListener);

        cancelButton = transform.transform.Find(Settings.ConstEditStoreMenuButtonCancel).gameObject;
        Button cancel = cancelButton.GetComponent<Button>();
        cancel.onClick.AddListener(ButtonsClickListener);
    }

    public void HideTopItem()
    {
        if (obj.Type == ObjectType.BASE_CONTAINER && obj != null)
        {
            spriteRenderer.color = new Color(0, 0, 0, 0);
        }
    }

    public void ShowTopItem()
    {
        if (obj.Type == ObjectType.BASE_CONTAINER && obj != null)
        {
            spriteRenderer.color = new Color(0, 0, 0, 1);
            spriteRenderer.color = Util.Available;
        }
    }

    public void SetTopItem(StoreGameObject obj)
    {
        if (obj.Type == ObjectType.BASE_CONTAINER)
        {
            spriteResolver.SetCategoryAndLabel(obj.SpriteLibCategory, obj.Identifier);
            ShowTopItem();
            ShowEditPanel();
            this.obj = obj;
        }
        else
        {
            GameLog.LogWarning("You can only set an item on top of a container");
        }
    }

    public StoreGameObject GetTopItem()
    {
        return obj;
    }

    public void ButtonsClickListener()
    {
        Debug.Log("Click listener ");
    }

    public void ShowEditPanel()
    {
        saveObjButton.SetActive(false);
        acceptButton.SetActive(false);
        cancelButton.SetActive(false);
    }

    public void HideEditPanel()
    {
        saveObjButton.SetActive(true);
        acceptButton.SetActive(true);
        cancelButton.SetActive(true);
    }
}