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
        public Vector3Surrogate finalPosition;
        public Vector3Surrogate finalAngles;
        
        //public ElementIdentifier kickerID;
        
        public Vector3Surrogate initialPositionBall;
        public Vector3Surrogate finalPositionBall;
        public bool ballCatched;
        public List<TargetForElement> targetsKickerID;
        
        public RecordData(GameObject element)
        {
            valid = true;
            layer = ActionsController.layer;

            idElement = 0;
            if (element.GetComponent<Player>())
                idElement = element.GetComponent<Player>().id;
            tagElement = element.tag;
            
            movements = new List<Vector3Surrogate>();
            angles = new List<Vector3Surrogate>();
            actions = new List<ActionsController.Azione>();
            initialPosition = new Vector3Surrogate(element.transform.position);
            initialRotation = new Vector3Surrogate(element.transform.eulerAngles);
                
            targetsKickerID = new List<TargetForElement>();
            ballCatched = false;
        }
        
        
        public Vector3 GetNextTargetForKicker(int kickerID, string kickerTag)
        {
            /*
             * Questo metodo è utilizzato per restituire il target di un determinato passaggio dato un determinato calciatore
             * 
             */
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
            /*
             * Resetto tutti i target
             * 
             */
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
            targetsKickerID = targets_kicker;
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
