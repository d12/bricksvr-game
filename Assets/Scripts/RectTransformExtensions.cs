using UnityEngine;

public static class RectTransformExtensions
{
    public static RectTransform SetLeft( this RectTransform rt, float x )
    {
        rt.offsetMin = new Vector2( x, rt.offsetMin.y );
        return rt;
    }
 
    public static RectTransform SetRight( this RectTransform rt, float x )
    {
        rt.offsetMax = new Vector2( -x, rt.offsetMax.y );
        return rt;
    }

    public static float GetRight(this RectTransform rt)
    {
        return -1 * rt.offsetMax.x;
    }
 
    public static RectTransform SetBottom( this RectTransform rt, float y )
    {
        rt.offsetMin = new Vector2( rt.offsetMin.x, y );
        return rt;
    }
 
    public static RectTransform SetTop( this RectTransform rt, float y )
    {
        rt.offsetMax = new Vector2( rt.offsetMax.x, -y );
        return rt;
    }
}