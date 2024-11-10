using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace AnarchyConstructFramework.Core.Common
{
    public class AnarchyData : ScriptableObject
    {
        // Dictionary to store previous field values for change detection
        private Dictionary<string, object> fieldValues = new Dictionary<string, object>();

        // Dictionary to hold UnityEvents for each field, to be linked to ConstructBindings
        private Dictionary<string, UnityEvent<object>> fieldEvents = new Dictionary<string, UnityEvent<object>>();

        // Method to set up change detection listeners for each field
        public void InitializeFieldListeners()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                // Store initial value in fieldValues for change detection
                fieldValues[field.Name] = field.GetValue(this);

                // Create a UnityEvent for the field if it doesn’t exist
                if (!fieldEvents.ContainsKey(field.Name))
                {
                    fieldEvents[field.Name] = new UnityEvent<object>();
                }
            }
        }

        // Method to check for field changes and invoke corresponding events
        public void DetectAndInvokeFieldChanges()
        {
            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                object currentValue = field.GetValue(this);
                
                // Check if the value has changed
                if (fieldValues.ContainsKey(field.Name) && !Equals(fieldValues[field.Name], currentValue))
                {
                    // Update the stored value
                    fieldValues[field.Name] = currentValue;

                    // Invoke the UnityEvent associated with the field
                    if (fieldEvents.TryGetValue(field.Name, out var fieldEvent))
                    {
                        fieldEvent.Invoke(currentValue);
                    }
                }
            }
        }

        // Method to access the UnityEvent for a specific field
        public UnityEvent<object> GetFieldEvent(string fieldName)
        {
            if (fieldEvents.TryGetValue(fieldName, out var fieldEvent))
            {
                return fieldEvent;
            }
            return null;
        }
    }
}
