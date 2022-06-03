using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
    public class RecordData
    {
        public bool valid;
        public int layer;
        
        public int idElement;
        public string tagElement;
        
        public List<Vector3Surrogate> movements;
        public List<Vector3Surrogate> angles;
        public List<ActionsController.Azione> actions;
        
        public Vector3Surrogate initialPosition;
        public Vector3Surrogate initialRotation;
        
        public List<TargetForElement> targetsKickerID;
        //public ElementIdentifier kickerID;
        public Vector3Surrogate finalPosition;
        public Vector3Surrogate finalAngles;

        public Vector3Surrogate initialPositionBall;
        public Vector3Surrogate finalPositionBall;
        public bool ballCatched;
        
        public RecordData(GameObject element)
        {
            valid = true;
            layer = ActionsController.layer;
            //this.element = element;
            if (element.GetComponent<Player>())
                idElement = element.GetComponent<Player>().id;
            else
                idElement = 0;
            
            tagElement = element.tag;
            movements = new List<Vector3Surrogate>();
            angles = new List<Vector3Surrogate>();
            actions = new List<ActionsController.Azione>();
            initialPosition = new Vector3Surrogate(element.transform.position);
            initialRotation = new Vector3Surrogate(element.transform.eulerAngles);
                
            targetsKickerID = new List<TargetForElement>();
            ballCatched = false;
        }

        private int indexNextTarget = 0;
        public int countChangeOwn = 0;
        
        public void SetNextTarget(int index)
        {
            indexNextTarget = index;
        }
        
        public Vector3 GetNextTargetForKicker(int kickerID, string kickerTag)
        {
            foreach (TargetForElement i in  targetsKickerID)
            {
                if (i.elementIdentifier.id == kickerID && i.elementIdentifier.tag.Equals(kickerTag) && !i.alreadyUsed)
                {
                    i.alreadyUsed = true;
                    return i.target.GetVector3();
                }
                    
            }
            return Vector3.zero;
        }

        public void ResetTargetForKicker()
        {
            foreach (TargetForElement i in targetsKickerID)
            {
                i.alreadyUsed = false;
            }
        }

        public void SetMovements(List<Vector3Surrogate> movements)
        {
            this.movements = movements;
        }

        public void SetAngles(List<Vector3Surrogate> angles)
        {
            this.angles = angles;
        }

        public void SetActions(List<ActionsController.Azione> actions)
        {
            this.actions = actions;
        }

        public void SetTargetsKicker(List<TargetForElement> targets_kicker)
        {
            this.targetsKickerID = targets_kicker;
        }

        public void SetFinalPosition(Vector3Surrogate finalPosition)
        {
            this.finalPosition = finalPosition;
        }

        public void SetFinalAngles(Vector3Surrogate finalAngles)
        {
            this.finalAngles = finalAngles;
        }
        
    }
