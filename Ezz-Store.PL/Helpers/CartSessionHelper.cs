using Ezz_Store.BLL.Models.Cart;
using Ezz_Store.PL.Extensions;

namespace Ezz_Store.PL.Helpers;

public static class CartSessionHelper
{
    private const string CartKey = "CART_SESSION";

    public static List<CartItemDto> GetCart(ISession session)
    {
        return session.GetObject<List<CartItemDto>>(CartKey) ?? [];
    }

    public static void SaveCart(ISession session, List<CartItemDto> items)
    {
        session.SetObject(CartKey, items);
    }

    public static void Clear(ISession session)
    {
        session.Remove(CartKey);
    }
}
