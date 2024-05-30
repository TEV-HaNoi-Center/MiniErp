using Microsoft.EntityFrameworkCore;
using MiniErp.Core.Repositories;
using MiniErp.DataAccess.Models;
using MiniErp.Domain;
using MiniErp.UI.ViewModels.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiniErp.UI.ViewModels
{
    public class InventoryViewModel : BaseViewModel
    {
        private readonly IRepository<ReceiveNoteDetail> _receiveRepository;
        private readonly IRepository<DeliveryNoteDetail> _deliveryRepository;

        private IEnumerable<InventoryModel> _details;
        public IEnumerable<InventoryModel> Details { get => _details; set { _details = value; OnPropertyChanged(); } }
        private InventoryModel _selectedItem;
        public InventoryModel SelectedItem { get => _selectedItem; set { _selectedItem = value; OnPropertyChanged(); } }
        public ICommand LoadCommand { get; set; }

        public InventoryViewModel(IRepository<ReceiveNoteDetail> receiveRepository, IRepository<DeliveryNoteDetail> deliveryRepository)
        {
            _receiveRepository = receiveRepository;
            _deliveryRepository = deliveryRepository;

            LoadCommand = new RelayCommand<object>(p => true, async p =>
            {
                var receives = _receiveRepository.AsQueryable().Include(x=>x.Product);
                var deliveries = _deliveryRepository.AsQueryable().Include(x=>x.Product);
                Details = await receives.GroupBy(x=>x.ProductId).Select(x=> new {ProductId = x.Key,Name = x.First().Product.Name, Code = x.First().Product.Code, Quantity = x.Sum(y=>y.Quantity)}).GroupJoin(deliveries, x => x.ProductId, y => y.ProductId, (x, y) =>
                new InventoryModel
                {
                    Code = x.Code,
                    Name = x.Name,
                    Quantity = x.Quantity - y.Sum(t => t.Quantity)
                }).ToListAsync();
            });
        }
    }
}
