using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timelogger
{
    public class ProjectManager
    {
        private String RootFolder = @"C:\Slimstock\Projekte\";

        public List<Project> Projects;

        private Project NotFound = new Project("kein Projekt");


        public ProjectManager()
        {
            Projects = new List<Project>();

            foreach (var item in Directory.GetDirectories(this.RootFolder, "*", SearchOption.TopDirectoryOnly))
            {
                Project p = new Project(item.Replace(RootFolder,""));
                p.RootDir = item;
                Projects.Add(p);
            }
        }


        public Project FindProject(Activity act)
        {
            foreach (var p in Projects)
            {
                if (p.Check(act))
                {
                    return p;
                }
            }
            return NotFound;

        }



    }
}
