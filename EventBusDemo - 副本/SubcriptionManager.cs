using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBudDemo
{
    public class SubcriptionManager
    {
        private readonly Dictionary<string, List<Type>> handle = new Dictionary<string, List<Type>>();
        public event EventHandler<string> OnEventRemoved;
        public bool IsEmpty =>!handle.Keys.Any();
        public void Clear() => handle.Clear();
        /// <summary>
        /// 注册事件的监听处理
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="handleType"></param>
        /// <exception cref="ArgumentException"></exception>
        public void AddSubcription(string eventName,Type handleType) 
        {
            if (!handle.ContainsKey(eventName))
            {
                handle[eventName] = new List<Type>();
            }
            if (handle[eventName].Contains(handleType))
            {
                throw new ArgumentException($"Handle type {handleType} already registered for '{eventName},'", nameof(handleType));
            }
            handle[eventName].Add(handleType);
        }
        /// <summary>
        /// 移除监听事件的处理类型
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="handleType"></param>
        public void UnSubcription(string eventName,Type handleType)
        {
            if (!handle.ContainsKey(eventName))
            {
                return;
            }
            if (handle[eventName].Contains(handleType))
            {
                handle[eventName].Remove(handleType);
            }
            if (!handle[eventName].Any())
            {
                handle.Remove(eventName);
                OnEventRemoved?.Invoke(this,eventName);
            }
        }
        /// <summary>
        /// 是否有监听eventName这个事件
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public bool HasScriptionForEvent(string eventName)
        {
            return handle.ContainsKey(eventName);
        }
        /// <summary>
        /// 得到名字为evtentName的事件监听者
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public IEnumerable<Type>GetHandlersEvent(string eventName)
        {
            return handle[eventName];
        }
    }
}
