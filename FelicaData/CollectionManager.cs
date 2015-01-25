using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelicaData
{
    public class CollectionManager
    {
        private readonly UserCollection users;
        private readonly CardCollection cards;
        private readonly MoneyHistoryCollection moneyHistories;
        private readonly UiPageCollection uiPages;
        private readonly UiTextCollection uiTexts;

        public CollectionManager(DatabaseManager mgr)
        {
            this.users = new UserCollection(mgr, this);
            this.cards = new CardCollection(mgr, this);
            this.moneyHistories = new MoneyHistoryCollection(mgr, this);
            this.uiPages = new UiPageCollection(mgr, this);
            this.uiTexts = new UiTextCollection(mgr, this);
        }

        public CardCollection Cards
        {
            get { return this.cards; }
        }

        public UserCollection Users
        {
            get { return this.users; }
        }

        public MoneyHistoryCollection MoneyHistories
        {
            get { return this.moneyHistories; }
        }

        public UiPageCollection UiPages
        {
            get { return this.uiPages; }
        }

        public UiTextCollection UiTexts
        {
            get { return this.uiTexts; }
        }
    }
}
