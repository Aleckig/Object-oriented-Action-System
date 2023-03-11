using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    
    private List<Vector3> positionList;
    private int currentPositionIndex;
    
    float stopDistance = 0.1f;
    
    [SerializeField] private float moveSpeed = 4.0f;
    [SerializeField] private float rotateSpeed = 10.0f;
    [SerializeField] private int maxMoveDistance = 5;
    [SerializeField] private Sprite moveActionIcon;
    

    void Update()
    {
        if(!isActive)
        {
            return;
        }

        Vector3 targetPosition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateSpeed*Time.deltaTime); //rotating the character to where the mousebutton was clicked
        
        if(Vector3.Distance(transform.position, targetPosition) > stopDistance) //preventing the player from jittering in place after moving
        {
            
            transform.position += moveDirection * moveSpeed *Time.deltaTime; // to not make the movment fps dependent
            

        }else
        {
            //check if all the movement has been done
            currentPositionIndex++;
            if(currentPositionIndex >= positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                ActionComplete();
            } 
        }     
    }

    //movent controlls for the players
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition>pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLenght); // implementing the pathfinding logic in the character movement
        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach(GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }    
        

        OnStartMoving?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);      
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        for(int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for(int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if(!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                if(unitGridPosition == testGridPosition)
                {
                    //same position wherer the unit is allowed
                    continue;
                }
                if(LevelGrid.Instance.HasAnyUnitonGridPosition(testGridPosition))
                {
                    //test if there is another unit on the same grid 
                    continue;
                }
                if(!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    //test if path  range is duable to "blocked" by obstacles 
                    continue;
                }
                if(!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition))
                {
                    //test if the path is even reacable
                    continue;
                }
                int pathfindingDistanceMultiplier = 10; //do to the grid values being multiplied by 10
                if(Pathfinding.Instance.GetPathLenght(unitGridPosition, testGridPosition) > maxMoveDistance *pathfindingDistanceMultiplier)
                {
                    //check if path lenght is to long
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;

    }

      public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        //checking if it is worth moving for the AI
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction 
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition *10, //serializefild this value in the future depending on the map size
        };
    }

    public override Sprite GetActionIcon()
    {
        return moveActionIcon;
    }


    
}
