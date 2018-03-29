using System.Collections.Generic;
using UnityEngine;
using System.Collections;
namespace CEngine
{
    public class BaseModel<T>
    {
        protected List<T> _datas;


        public BaseModel()
        {
            _datas = new List<T>();
        }


        public void Init()
        {
            _datas = new List<T>();
        }

        public void Add(T data)
        {
            if (!_datas.Contains(data))
                _datas.Add(data);
        }


        public List<T> GetAllData()
        {
            return _datas;
        }

        public void Update(T _new)
        {
            if (_datas.Contains(_new))
                Update(_datas.IndexOf(_new), _new);
            else
                Add(_new);
        }

        public void Update(int index, T _new)
        {
            if (index < _datas.Count && index >= 0)
                _datas[index] = _new;
        }

        public void Remove(int index)
        {
            if (index < _datas.Count && index >= 0)
                _datas.RemoveAt(index);
        }

        public void Remove(T t)
        {
            if (_datas.Contains(t))
                Remove(t);
        }

        public void Merge(List<T> lstNew)
        {
            if (lstNew == null) return;
            for (int i = 0; i < lstNew.Count; i++)
            {
                if (!_datas.Contains(lstNew[i]))
                {
                    _datas.Add(lstNew[i]);
                }
            }

        }

        public void CustomSort()
        {
            if (_datas.Count == 0)
                return;
            _datas.Sort();
        }

        public void CustomSort(System.Comparison<T> comp)
        {
            if (_datas.Count == 0)
                return;
            if (comp == null)
                return;
            _datas.Sort(comp);
        }

        public void CustomSort(IComparer<T> comparer)
        {
            if (_datas.Count == 0)
                return;
            if (comparer == null)
                return;
            _datas.Sort(comparer);
        }
        public void Clear()
        {
            if (_datas == null)
                return;
            _datas.Clear();
        }



    }
}