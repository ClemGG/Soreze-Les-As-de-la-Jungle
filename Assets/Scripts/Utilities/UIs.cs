using UnityEngine;

public static class UIs
{

    /* Les rects sont en local par défaut vu qu'ils sont enfants du canvas, cette fonction permet les Contains() correctement, mais pas les Overlaps()
     * 
     * La méthode Contains() fonctionne de la façon :
     * if(Rect leRectContenant.GetWorldSapceRect().Contains(Vector2 leRectSuperposé.center)) { //Le Contains() fonctionne ici }
     */

    public static Rect GetWorldSapceRect(this RectTransform rt)
    {
        var r = rt.rect;
        r.center = rt.TransformPoint(r.center);
        r.size = rt.TransformVector(r.size);
        return r;
    }

    /*Crée une version réduite et centrée du rect d'origine.
     * Chaque côté du rect sera réduit de la moitié du rectSizeOffset
     */
    public static Rect GetWorldSapceRect(this RectTransform rt, Vector2 rectSizeOffset)
    {
        var r = rt.rect;
        r.center = rt.TransformPoint(r.center + rectSizeOffset / 2f);
        r.size = rt.TransformVector(r.size - rectSizeOffset);
        return r;
    }

    public static Vector3 ClampedInsideOfRect(this Vector3 unclampedPos, Rect rt)
    {
        Vector3 v3 = unclampedPos;

        v3.x = v3.x < rt.xMin ? rt.xMin : v3.x > rt.xMax ? rt.xMax : v3.x;
        v3.y = v3.y < rt.yMin ? rt.yMin : v3.y > rt.yMax ? rt.yMax : v3.y;

        return v3;
    }

}
