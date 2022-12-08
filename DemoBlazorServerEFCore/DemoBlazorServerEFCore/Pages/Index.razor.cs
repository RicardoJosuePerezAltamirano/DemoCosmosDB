using DemoBlazorServerEFCore.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace DemoBlazorServerEFCore.Pages
{
    public partial class Index
    {
        [Inject]
        public IDbContextFactory<DemoContext> ContextFactory { get; set; }
        public List<Users> UsersData { get; set; }

        public string Name { get; set; }
        protected override async Task OnInitializedAsync()
        {
            using (var Context = await ContextFactory.CreateDbContextAsync())
            {
                await GetData(Context);
            }
                
            
           
        }
        public async Task Save()
        {
            Users user = new Users()
            {
                Name = Name
            };
            using (var Context = await ContextFactory.CreateDbContextAsync())
            {


                await Context.Users.AddAsync(user);
                await Context.SaveChangesAsync();
                await GetData(Context);
            }
            
        }
        private async Task GetData(DemoContext Context)
        {
            UsersData = await Context.Users.ToListAsync();
        }
    }

}
