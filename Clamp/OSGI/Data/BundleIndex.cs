using Clamp.OSGI.Data.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data
{
    class BundleIndex
    {
        Dictionary<string, List<BundleDescription>> addins = new Dictionary<string, List<BundleDescription>>();

        public void Add(BundleDescription desc)
        {
            string id = Bundle.GetFullId(desc.Namespace, desc.LocalId, null);
            List<BundleDescription> list;
            if (!addins.TryGetValue(id, out list))
                addins[id] = list = new List<BundleDescription>();
            list.Add(desc);
        }

        public IEnumerable<string> GetMissingDependencies(BundleDescription desc, ModuleDescription mod)
        {
            foreach (Dependency dep in mod.Dependencies)
            {
                BundleDependency adep = dep as BundleDependency;
                if (adep == null)
                    continue;
                var descs = FindDescriptions(desc.Domain, adep.FullBundleId);
                if (descs.Count == 0)
                    yield return adep.FullBundleId;
            }
        }

        public BundleDescription GetSimilarExistingBundle(BundleDescription conf, string addinId)
        {
            string domain = conf.Domain;
            List<BundleDescription> list;
            if (!addins.TryGetValue(Bundle.GetIdName(addinId), out list))
                return null;
            string version = Bundle.GetIdVersion(addinId);
            foreach (BundleDescription desc in list)
            {
                if ((desc.Domain == domain || domain == BundleDatabase.GlobalDomain) && !desc.SupportsVersion(version))
                    return desc;
            }
            return null;
        }

        public string FindCondition(BundleDescription desc, ModuleDescription mod, string conditionId)
        {
            if (desc.ConditionTypes.Any(c => c.Id == conditionId))
                return desc.BundleId;

            foreach (Dependency dep in mod.Dependencies)
            {
                BundleDependency adep = dep as BundleDependency;

                if (adep == null)
                    continue;
                var descs = FindDescriptions(desc.Domain, adep.FullBundleId);
                foreach (var d in descs)
                {
                    var c = FindCondition(d, d.MainModule, conditionId);
                    if (c != null)
                        return c;
                }
            }
            return null;
        }

        public List<BundleDescription> GetSortedBundles()
        {
            var inserted = new HashSet<string>();
            var lists = new Dictionary<string, List<BundleDescription>>();

            foreach (List<BundleDescription> dlist in addins.Values)
            {
                foreach (BundleDescription desc in dlist)
                    InsertSortedBundle(inserted, lists, desc);
            }

            // Merge all domain lists into a single list.
            // Make sure the global domain is inserted the last

            List<BundleDescription> global;

            lists.TryGetValue(BundleDatabase.GlobalDomain, out global);
            lists.Remove(BundleDatabase.GlobalDomain);

            List<BundleDescription> sortedBundles = new List<BundleDescription>();

            foreach (var dl in lists.Values)
            {
                sortedBundles.AddRange(dl);
            }

            if (global != null)
                sortedBundles.AddRange(global);

            return sortedBundles;
        }

        List<BundleDescription> FindDescriptions(string domain, string fullid)
        {
            // Returns all registered add-ins which are compatible with the provided
            // fullid. Compatible means that the id is the same and the version is within
            // the range of compatible versions of the add-in.

            var res = new List<BundleDescription>();
            string id = Bundle.GetIdName(fullid);
            List<BundleDescription> list;
            if (!addins.TryGetValue(id, out list))
                return res;
            string version = Bundle.GetIdVersion(fullid);
            foreach (BundleDescription desc in list)
            {
                if ((desc.Domain == domain || domain == BundleDatabase.GlobalDomain) && desc.SupportsVersion(version))
                    res.Add(desc);
            }
            return res;
        }


        void InsertSortedBundle(HashSet<string> inserted, Dictionary<string, List<BundleDescription>> lists, BundleDescription desc)
        {
            string sid = desc.BundleId + " " + desc.Domain;
            if (!inserted.Add(sid))
                return;

            foreach (ModuleDescription mod in desc.AllModules)
            {
                foreach (Dependency dep in mod.Dependencies)
                {
                    BundleDependency adep = dep as BundleDependency;
                    if (adep == null)
                        continue;
                    var descs = FindDescriptions(desc.Domain, adep.FullBundleId);
                    if (descs.Count > 0)
                    {
                        foreach (BundleDescription sd in descs)
                            InsertSortedBundle(inserted, lists, sd);
                    }
                }
            }
            List<BundleDescription> list;
            if (!lists.TryGetValue(desc.Domain, out list))
                lists[desc.Domain] = list = new List<BundleDescription>();

            list.Add(desc);
        }

    }
}
