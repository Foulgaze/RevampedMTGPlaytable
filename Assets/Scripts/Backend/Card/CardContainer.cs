using System;
using System.Collections.Generic;
using System.ComponentModel;

public class CardContainer
{
 
    public CardZone Zone { get; }


    public List<Card> Cards { get; }

    public string Owner { get; }

    /// <summary>
    /// Occurs when the cards collection changes.
    /// </summary>
    public event PropertyChangedEventHandler cardsChanged = delegate { };

    /// <summary>
    /// Initializes a new instance of the <see cref="CardContainer"/> class with the specified zone and owner.
    /// </summary>
    /// <param name="zone">The zone of the card container.</param>
    /// <param name="owner">The owner of the card container.</param>
    public CardContainer(CardZone zone, string owner)
    {
        this.Zone = zone;
        this.Owner = owner;
    }

    /// <summary>
    /// Adds a card to the container at the specified position.
    /// </summary>
    /// <param name="card">The card to add.</param>
    /// <param name="position">The position to insert the card at. If null, the card is added to the end.</param>
    /// <param name="networkChange">If set to <c>true</c>, triggers the <see cref="cardsChanged"/> event.</param>
    public void AddCardToContainer(Card card, int? position, bool networkChange)
    {
        int insertPosition = position == null ? Cards.Count : position.Value;
        Cards.Insert(insertPosition, card);
        if (networkChange)
        {
            cardsChanged(this, new PropertyChangedEventArgs("added"));
        }
    }

    /// <summary>
    /// Removes a card from the container.
    /// </summary>
    /// <param name="card">The card to remove.</param>
    /// <param name="networkChange">If set to <c>true</c>, triggers the <see cref="cardsChanged"/> event.</param>
    public void RemoveCardFromContainer(Card card, bool networkChange)
    {
        if (!Cards.Remove(card) || !networkChange)
        {
            return;
        }
        cardsChanged(this, new PropertyChangedEventArgs("removed"));
    }

    /// <summary>
    /// Gets the name of the card zone.
    /// </summary>
    /// <returns>The name of the card zone.</returns>
    public string GetName()
    {
        return Enum.GetName(typeof(CardZone), Zone);
    }
}
