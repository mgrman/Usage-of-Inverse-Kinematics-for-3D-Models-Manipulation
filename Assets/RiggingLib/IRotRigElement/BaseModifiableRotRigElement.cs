using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

   public abstract class BaseModifiableRotRigElement: IRotRigElement
    {
        protected List<IRotRigModifier> _modifs = new List<IRotRigModifier>();

        public void AddModifier(IRotRigModifier modif)
        {
            _modifs.Add(modif);
        }


        public IEnumerable< Quaternion> UpdateElement(bool useLocal)
        {
            IEnumerable<Quaternion> rotations = UpdateElementImpl(useLocal);
            foreach (var modif in _modifs)
            {
                rotations = modif.UpdateElement(rotations,useLocal);
            }
            return rotations;
        }


        public abstract Transform[] BoundObjects { get; }

        protected abstract Quaternion[] UpdateElementImpl(bool useLocal);
    }

