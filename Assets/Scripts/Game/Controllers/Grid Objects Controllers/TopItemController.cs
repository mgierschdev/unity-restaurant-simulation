using UnityEngine;
using UnityEngine.U2D.Animation;

// Control the top item movement between gamegrid containers
// this script is attached to the base -> BaseObject/Object/TopObject
public class TopItemController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private SpriteResolver spriteResolver;
    private StoreGameObject obj;

    public void Start()
    {
        obj = null;
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        spriteResolver = transform.GetComponent<SpriteResolver>();
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
}