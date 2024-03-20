using UnityEngine;

public interface CardContainer
{
    public void AddCardToContainer(Card card, int? position);
    public void RemoveCardFromContainer(Card card);

    public int GetOwner();
}