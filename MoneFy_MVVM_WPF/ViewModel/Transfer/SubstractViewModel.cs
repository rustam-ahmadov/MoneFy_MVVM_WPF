﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using MoneFy_MVVM_WPF.Enums;
using MoneFy_MVVM_WPF.Messages;
using MoneFy_MVVM_WPF.Model;
using MoneFy_MVVM_WPF.Services;
using MoneFy_MVVM_WPF.ViewModel.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MoneFy_MVVM_WPF.ViewModel.Transfer
{

    public class SubstractViewModel : ViewModelBase
    {
        private readonly IMessenger messanger;
        private readonly INavigationService navigationService;
        private readonly ITransactionService transactionService;
        private ViewModelBase _categoryView;
        private Transaction transaction = App.Container.GetInstance<Transaction>();
        public ViewModelBase CategoryView
        {
            get => _categoryView;
            set
            {
                Set(ref _categoryView, value);
            }
        }
        private bool _check;
        public void CloseOpenCategory(bool _check, ViewModelBase viewModel)
        {
            if (_check == false)
            {
                CategoryView = viewModel;
                _check = true;
            }
            else
            {
                CategoryView = null;
                _check = false;
            }
        }
        public SubstractViewModel(IMessenger M, INavigationService NS, ITransactionService TS)
        {
            messanger = M;
            navigationService = NS;
            transactionService = TS;
            messanger.Register<NavigationMessage>(this, NavToken.Category, messanger =>
            {
                ViewModelBase viewModel = App.Container.GetInstance(messanger.ViewModelBase) as ViewModelBase;
                CloseOpenCategory(this._check, viewModel);
            });
            messanger.Register<TransactionMessage>(this, AccToken.Substract, messanger =>
            {
                Money = "";
                CategoryView = null;
            }
            );
        }

        string _money;
        public string Money
        {
            get => _money;
            set
            {
                Set(ref _money, value);
                MoneyChange();
            }
        }
        bool _acptButEnab;
        public bool AcptButEnab
        {
            get => _acptButEnab;
            set
            {
                Set(ref _acptButEnab, value);
            }
        }

        #region Commands
        private RelayCommand _enter;
        public RelayCommand Enter
        {
            get => _enter ??= new(delegate { CategoryView = null; });
        }
        private RelayCommand _toHome;
        public RelayCommand ToHome
        {
            get => _toHome ??= new RelayCommand(delegate
            {
                if (Money != null)
                    Money = "";
                this.CategoryView = null;
                navigationService.NavigateTo<HomeViewModel>(NavToken.Main);
            });
        }
        private RelayCommand _accept;
        public RelayCommand Accept
        {
            get => _accept ??= new RelayCommand(delegate
            {

                transaction.Summ = double.Parse(Money);
                transactionService.Transact(transaction, AccToken.Category);
                navigationService.NavigateTo<CategoryViewModel>(NavToken.Category);
            });
        }
        #endregion

        public void MoneyChange()
        {

            if (_money != null && _money != "")
            {
                if (double.Parse(_money) > 0)
                    AcptButEnab = true;
            }
            else
                AcptButEnab = false;
        }

    }
}
