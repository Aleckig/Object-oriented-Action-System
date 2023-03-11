using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//edit -->project settings ---> script order, this script is the first to run.
public class UnitActionSystem : MonoBehaviour
{
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;
    
    public static UnitActionSystem Instance{get; private set;} // can be read by all classes but only edited by this 
   
    [SerializeField] private Squad selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;
    
    private bool isBusy;
    private BaseAction selectedAction;

   

    private void Awake() 
    {   
        if(Instance != null)
        {
            //destroy objects if there is more then one 
            Destroy(gameObject);
            return;
        }
        Instance = this;
    
    }
    
    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }

   private void Update()
   {    
        
        if(isBusy)
        {
            return;
        }

        //checking if it is enemy turn
        if(!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }
        //moving the player character to the mouse position
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
            
        }
        if(TryHandleUnitSelection())
        {
            return;
        }
        HandleSelectedAction();

   }
   private void HandleSelectedAction()
   {    
        if (InputManager.Instance.IsMouseButtonDownThisFrame())

        {
           GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
           if(!selectedAction.IsValidActionGridPosition(mouseGridPosition))
           {
                return;
           }
           
            if(!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
            {
               return;     
            }
            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);
            OnActionStarted?.Invoke(this, EventArgs.Empty); 
                
           
           
        }

   }
   private void SetBusy()
   {
        isBusy = true ;

        OnBusyChanged?.Invoke(this, isBusy); 
   }

   private void ClearBusy()
   {
        isBusy = false;

        OnBusyChanged?.Invoke(this, isBusy); 
   }

   private bool TryHandleUnitSelection()
   {    
    //with the mouse button trying to select player Units
       if (InputManager.Instance.IsMouseButtonDownThisFrame())

        {
            
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());

            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                
                if (raycastHit.transform.TryGetComponent<Squad>(out Squad unit))
                {
                    if (unit == selectedUnit)
                    {
                        // Unit is already selected
                        return false;
                    }

                    if(unit.IsEnemy())
                    {
                        //if player clicked an enemy
                        return false;
                    }
                    
                    SetSelectedUnit(unit);
                    return true;
                }
            }
            
        }
        return false;

   }
   private void SetSelectedUnit(Squad unit)
   {
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());

        //activating the event 
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);

   }

   public void SetSelectedAction(BaseAction baseAction)
   {
        selectedAction = baseAction;

        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
   }

    //Exposing the current unit
   public Squad GetSelectedUnit()
   {
        return selectedUnit;
   }
   public BaseAction GetSelectedAction()
   {
        return selectedAction;
   }

   
}
