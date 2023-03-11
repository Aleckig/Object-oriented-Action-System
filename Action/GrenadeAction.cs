using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    //Do not add sound source in this script!

    [SerializeField] private int maxThrowDistance = 7;
    [SerializeField] private Transform grenadeProjectilePrefab;
    [SerializeField] private Sprite grenadeActionIcon;
    [SerializeField] private LayerMask obstacleLayerMask;
    

     private void Update()
    {
        if (!isActive)
        {
            return;
        }
    }


    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxThrowDistance)
                {
                    continue;
                }
                Vector3 unitWorldPosition = unit.GetWorldPosition();
                Vector3 targetWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                Vector3 throwDir = (targetWorldPosition - unitWorldPosition).normalized;

                if (Physics.Raycast(unitWorldPosition, throwDir, Vector3.Distance(unitWorldPosition, targetWorldPosition), obstacleLayerMask))
                {
                // Blocked by an obstacle, don't throw the grenade
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }


    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
      
        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviourComplete);

        

        ActionStart(onActionComplete);
    }

    private void OnGrenadeBehaviourComplete()
    {
        ActionComplete();
    }

    public override Sprite GetActionIcon()
    {
        return grenadeActionIcon;
    }

}
