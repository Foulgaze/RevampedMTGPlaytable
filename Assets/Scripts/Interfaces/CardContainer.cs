using UnityEngine;
// TO DO
// This should probably be made an abstract class instead. 
public interface CardContainer
{
    public void AddCardToContainer(Card card, int? position);
    public void RemoveCardFromContainer(Card card);

    public int GetOwner();

    public void ReleaseCardInBox(Card card);

}