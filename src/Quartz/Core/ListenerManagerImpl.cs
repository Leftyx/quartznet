using System;
using System.Collections.Generic;

using Quartz.Impl.Matchers;
using Quartz.Util;

namespace Quartz.Core
{
    public class ListenerManagerImpl : IListenerManager
    {
        private readonly Dictionary<string, IJobListener> globalJobListeners = new Dictionary<string, IJobListener>(10);

        private readonly Dictionary<string, ITriggerListener> globalTriggerListeners = new Dictionary<string, ITriggerListener>(10);

        private readonly Dictionary<string, List<IMatcher<JobKey>>> globalJobListenersMatchers = new Dictionary<string, List<IMatcher<JobKey>>>(10);

        private readonly Dictionary<string, List<IMatcher<TriggerKey>>> globalTriggerListenersMatchers = new Dictionary<string, List<IMatcher<TriggerKey>>>(10);

        private readonly List<ISchedulerListener> schedulerListeners = new List<ISchedulerListener>(10);


        public void AddJobListener(IJobListener jobListener, params IMatcher<JobKey>[] matchers)
        {
            AddJobListener(jobListener, new List<IMatcher<JobKey>>(matchers));
        }

        public void AddJobListener(IJobListener jobListener, IList<IMatcher<JobKey>> matchers)
        {
            if (String.IsNullOrEmpty(jobListener.Name))
            {
                throw new ArgumentException(
                    "JobListener name cannot be empty.");
            }

            lock (globalJobListeners)
            {
                globalJobListeners[jobListener.Name] = jobListener;
                if (matchers == null)
                {
                    matchers = new List<IMatcher<JobKey>>();
                }
                if (matchers.Count == 0)
                {
                    matchers.Add(EverythingMatcher<JobKey>.AllJobs());
                }
                globalJobListenersMatchers[jobListener.Name] = new List<IMatcher<JobKey>>(matchers);
            }
        }


        public bool AddJobListenerMatcher(string listenerName, IMatcher<JobKey> matcher)
        {
            if (matcher == null)
            {
                throw new ArgumentException("Non-null value not acceptable.");
            }

            lock (globalJobListeners)
            {
                IList<IMatcher<JobKey>> matchers = globalJobListenersMatchers.TryGetAndReturn(listenerName);
                if (matchers == null)
                {
                    return false;
                }
                matchers.Add(matcher);
                return true;
            }
        }

        public bool RemoveJobListenerMatcher(string listenerName, IMatcher<JobKey> matcher)
        {
            if (matcher == null)
            {
                throw new ArgumentException("Non-null value not acceptable.");
            }

            lock (globalJobListeners)
            {
                IList<IMatcher<JobKey>> matchers = globalJobListenersMatchers.TryGetAndReturn(listenerName);
                if (matchers == null)
                {
                    return false;
                }
                return matchers.Remove(matcher);
            }
        }

        public IList<IMatcher<JobKey>> GetJobListenerMatchers(string listenerName)
        {
            lock (globalJobListeners)
            {
                List<IMatcher<JobKey>> matchers = globalJobListenersMatchers.TryGetAndReturn(listenerName);
                if (matchers == null)
                {
                    return null;
                }
                return matchers.AsReadOnly();
            }
        }

        public bool SetJobListenerMatchers(string listenerName, IList<IMatcher<JobKey>> matchers)
        {
            if (matchers == null)
            {
                throw new ArgumentException("Non-null value not acceptable.");
            }

            lock (globalJobListeners)
            {
                List<IMatcher<JobKey>> oldMatchers = globalJobListenersMatchers.TryGetAndReturn(listenerName);
                if (oldMatchers == null)
                {
                    return false;
                }
                globalJobListenersMatchers[listenerName] = new List<IMatcher<JobKey>>(matchers);
                return true;
            }
        }


        public bool RemoveJobListener(string name)
        {
            lock (globalJobListeners)
            {
                return globalJobListeners.Remove(name);
            }
        }

        public IList<IJobListener> GetJobListeners()
        {
            lock (globalJobListeners)
            {
                return new List<IJobListener>(globalJobListeners.Values).AsReadOnly();
            }
        }

        public IJobListener GetJobListener(string name)
        {
            lock (globalJobListeners)
            {
                return globalJobListeners.TryGetAndReturn(name);
            }
        }

        public void AddTriggerListener(ITriggerListener triggerListener, params IMatcher<TriggerKey>[] matchers)
        {
            AddTriggerListener(triggerListener, new List<IMatcher<TriggerKey>>(matchers));
        }

        public void AddTriggerListener(ITriggerListener triggerListener, IList<IMatcher<TriggerKey>> matchers)
        {
            if (String.IsNullOrEmpty(triggerListener.Name))
            {
                throw new ArgumentException("TriggerListener name cannot be empty.");
            }

            lock (globalTriggerListeners)
            {
                globalTriggerListeners[triggerListener.Name] = triggerListener;
                if (matchers == null)
                {
                    matchers = new List<IMatcher<TriggerKey>>();
                }
                if (matchers.Count == 0)
                {
                    matchers.Add(EverythingMatcher<TriggerKey>.AllTriggers());
                }
                globalTriggerListenersMatchers[triggerListener.Name] = new List<IMatcher<TriggerKey>>(matchers);
            }
        }

        public void AddTriggerListener(ITriggerListener triggerListener, IMatcher<TriggerKey> matcher)
        {
            if (matcher == null)
            {
                throw new ArgumentException("Non-null value not acceptable for matcher.");
            }

            if (String.IsNullOrEmpty(triggerListener.Name))
            {
                throw new ArgumentException("TriggerListener name cannot be empty.");
            }

            lock (globalTriggerListeners)
            {
                globalTriggerListeners[triggerListener.Name] = triggerListener;
                var matchers = new List<IMatcher<TriggerKey>> {matcher};
                globalTriggerListenersMatchers[triggerListener.Name] = matchers;
            }
        }

        public bool AddTriggerListenerMatcher(string listenerName, IMatcher<TriggerKey> matcher)
        {
            if (matcher == null)
            {
                throw new ArgumentException("Non-null value not acceptable.");
            }

            lock (globalTriggerListeners)
            {
                IList<IMatcher<TriggerKey>> matchers = globalTriggerListenersMatchers.TryGetAndReturn(listenerName);
                if (matchers == null)
                {
                    return false;
                }
                matchers.Add(matcher);
                return true;
            }
        }

        public bool RemoveTriggerListenerMatcher(string listenerName, IMatcher<TriggerKey> matcher)
        {
            if (matcher == null)
            {
                throw new ArgumentException("Non-null value not acceptable.");
            }

            lock (globalTriggerListeners)
            {
                IList<IMatcher<TriggerKey>> matchers = globalTriggerListenersMatchers.TryGetAndReturn(listenerName);
                if (matchers == null)
                {
                    return false;
                }
                return matchers.Remove(matcher);
            }
        }

        public IList<IMatcher<TriggerKey>> GetTriggerListenerMatchers(string listenerName)
        {
            lock (globalTriggerListeners)
            {
                List<IMatcher<TriggerKey>> matchers = globalTriggerListenersMatchers.TryGetAndReturn(listenerName);
                if (matchers == null)
                {
                    return null;
                }
                return matchers;
            }
        }

        public bool SetTriggerListenerMatchers(string listenerName, IList<IMatcher<TriggerKey>> matchers)
        {
            if (matchers == null)
            {
                throw new ArgumentException("Non-null value not acceptable.");
            }

            lock (globalTriggerListeners)
            {
                IList<IMatcher<TriggerKey>> oldMatchers = globalTriggerListenersMatchers.TryGetAndReturn(listenerName);
                if (oldMatchers == null)
                {
                    return false;
                }
                globalTriggerListenersMatchers[listenerName] = new List<IMatcher<TriggerKey>>(matchers);
                return true;
            }
        }

        public bool RemoveTriggerListener(string name)
        {
            lock (globalTriggerListeners)
            {
                return globalTriggerListeners.Remove(name);
            }
        }


        public IList<ITriggerListener> GetTriggerListeners()
        {
            lock (globalTriggerListeners)
            {
                return new List<ITriggerListener>(globalTriggerListeners.Values).AsReadOnly();
            }
        }

        public ITriggerListener GetTriggerListener(string name)
        {
            lock (globalTriggerListeners)
            {
                return globalTriggerListeners.TryGetAndReturn(name);
            }
        }


        public void AddSchedulerListener(ISchedulerListener schedulerListener)
        {
            lock (schedulerListeners)
            {
                schedulerListeners.Add(schedulerListener);
            }
        }

        public bool RemoveSchedulerListener(ISchedulerListener schedulerListener)
        {
            lock (schedulerListeners)
            {
                return schedulerListeners.Remove(schedulerListener);
            }
        }

        public IList<ISchedulerListener> GetSchedulerListeners()
        {
            lock (schedulerListeners)
            {
                return schedulerListeners.AsReadOnly();
            }
        }
    }
}