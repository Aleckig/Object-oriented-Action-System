using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealingAction : BaseAction
{
    [SerializeField] private int maxThrowDistance = 7;
    [SerializeField] private Transform healingProjectilePrefab;
    [SerializeField] private Sprite healingActionIcon;
    [SerializeField] private LayerMask obstacleLayerMask;//

     private void Update()
    {
        if (!isActive)
        {
            return;
        }
    }


    public override string GetActionName()
    {
        return "Heal";
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

    /*public override List<GridPosition> GetValidActionGridPositionList()
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

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    } */

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform healingProjectileTransform = Instantiate(healingProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        HealingProjectile healingProjectile = healingProjectileTransform.GetComponent<HealingProjectile>();
        healingProjectile.Setup(gridPosition, OnHealingBehaviourComplete);

        ActionStart(onActionComplete);
    }

    private void OnHealingBehaviourComplete()
    {
        ActionComplete();
    }

    public override Sprite GetActionIcon()
    {
        return healingActionIcon;
    }
}
