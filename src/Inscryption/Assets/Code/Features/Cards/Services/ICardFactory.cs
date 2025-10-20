using Code.Features.Cards.Data;
using UnityEngine;

namespace Code.Features.Cards.Services
{
    public interface ICardFactory
    {
        GameEntity CreateCard(CardCreateData cardCreateData);
        GameEntity CreateRandomCard(CardCreateData cardCreateData);
    }
}

