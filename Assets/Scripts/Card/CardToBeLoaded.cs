using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;
public class CardTexture
{
    public Queue<ITextureable> cardsToBeTextured {get;} = new Queue<ITextureable>();
    public CardInfo info {get;}

    public CardTexture(CardInfo info)
    {
        this.info = info;
    }

    public void Enqueue(ITextureable textureable)
    {
        cardsToBeTextured.Enqueue(textureable);
    }

    public ITextureable Dequeue()
    {
        return cardsToBeTextured.Dequeue();
    }
}

public interface ITextureable
{
    public void TextureSelf(CardInfo info);
    public CardInfo GetInfo();
}

