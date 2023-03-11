using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractAction : BaseAction
{
    
    [SerializeField] private Sprite interactionActionIcon;
    [SerializeField] private AudioClip shootSoundClip;
    
    private int maxInteractDistance = 1;
    private AudioSource audioSource;
    //change ap cost to 0 

     private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
    }
    private void Update()
    {
        if (!isActive)
        {
            return;
        }
    }

    public override string GetActionName()
    {
        return "Interact"; //Interact, Use
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        //checking the grid of there is an Interactive object.
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++)
        {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);

                if (interactable == null)
                {
                    // No Door on this GridPosition
                    // No interactable on this GridPosition
                    continue;
                }


                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);

        interactable.Interact(OnInteractComplete);


        ActionStart(onActionComplete);
    }

    private void OnInteractComplete()
    {
        audioSource.PlayOneShot(shootSoundClip);//
        ActionComplete();
    }
    public override Sprite GetActionIcon()
    {
        return interactionActionIcon;
    }

}
