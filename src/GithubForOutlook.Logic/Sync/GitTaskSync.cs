using System;
using System.Collections.Generic;
using VSTOContrib.Outlook.Services;

namespace GithubForOutlook.Logic.Sync
{
    public class GitTaskSynchronisationService
    {
        public void Synchronise()
        {
            var syncService = new GenericSynchronisationService<IGithubIssue, int>(
                i=>i.Id,
                new OutlookSyncProvider(),
                new GithubSyncProvider(),
                new GithubSyncSettings(),
                SyncDirection.TwoWay);
        }
    }

    /// <summary>
    /// TODO, this class holds the state about when last sync was.
    /// 
    /// You need to stash/retreive the lastsync, and when save is called is a hook to persist
    /// </summary>
    public class GithubSyncSettings : ISyncSettings
    {
        public void Save()
        {
            
        }

        public DateTime? LastSync
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }


    //TODO use http://vstocontrib.codeplex.com/SourceControl/changeset/view/2b49690714a8#src%2fDemoProjects%2fFacebookToOutlook%2fFacebookToOutlookCore%2fRepositories%2fOutlookDispatchingRepository.cs 
    // as a sample implementation

    // Use the outlook dispatching repository. Otherwise if sync is on the background it marshals
    // to outlooks STA thread every time you access a task, making the sync super slow, 
    //and killing outlooks perf while the sync happens
    //

    public class GithubSyncProvider : ISynchronisationProvider<IGithubIssue, int>
    {
        public IEnumerable<IGithubIssue> GetModifiedEntries(DateTime? lastSync)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetDeletedEntries(DateTime? lastSync)
        {
            throw new NotImplementedException();
        }

        public void SaveEntries(IEnumerable<IGithubIssue> entries)
        {
            throw new NotImplementedException();
        }

        public void DeleteEntries(IEnumerable<int> keys)
        {
            throw new NotImplementedException();
        }
    }

    public class OutlookSyncProvider : ISynchronisationProvider<IGithubIssue, int>
    {
        public IEnumerable<IGithubIssue> GetModifiedEntries(DateTime? lastSync)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> GetDeletedEntries(DateTime? lastSync)
        {
            throw new NotImplementedException();
        }

        public void SaveEntries(IEnumerable<IGithubIssue> entries)
        {
            throw new NotImplementedException();
        }

        public void DeleteEntries(IEnumerable<int> keys)
        {
            throw new NotImplementedException();
        }
    }

    public interface IGithubIssue
    {
        int Id { get; set; }
    }
}