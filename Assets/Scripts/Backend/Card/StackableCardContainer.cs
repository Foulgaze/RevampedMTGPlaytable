public class StackableCardContainer : CardContainer
{
    private List<CardContainer> cardContainers = new List<CardContainer>();
    public StackableCardContainer(CardZone zone, string owner) : card base(zone, owner)
    {
        
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

}