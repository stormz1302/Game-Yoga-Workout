using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Samples.Purchasing.Core.BuyingConsumables
{
    public class ShopPurchase : MonoBehaviour, IStoreListener
    {
        IStoreController m_StoreController; // The Unity Purchasing system.

        //Your products IDs. They should match the ids of your products in your store.
        //string diamondProductId1 = "com.DefaultCompany.YogaWorkout.4D";
        string removeAdsProductId;
        //string diamondProductId2;
        //string diamondProductId3;
        //string diamondProductId4;
        //string diamondProductId5;
        //string diamondProductId6;

        public static ShopPurchase instance;
        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            removeAdsProductId = Application.identifier + ".removeAds";
        }

        void Start()
        {
            //diamondProductId1 = GameData.ID_IAP_DIAMOND1;
            //diamondProductId2 = GameData.ID_IAP_DIAMOND2;
            //diamondProductId3 = GameData.ID_IAP_DIAMOND3;
            //diamondProductId4 = GameData.ID_IAP_DIAMOND4;
            //diamondProductId5 = GameData.ID_IAP_DIAMOND5;
            //diamondProductId6 = GameData.ID_IAP_DIAMOND6;


            InitializePurchasing();
        }

        void InitializePurchasing()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            //Add products that will be purchasable and indicate its type.
            //builder.AddProduct(diamondProductId1, ProductType.Consumable);
            builder.AddProduct(removeAdsProductId, ProductType.NonConsumable);
            /* builder.AddProduct(diamondProductId2, ProductType.Consumable);
             builder.AddProduct(diamondProductId3, ProductType.Consumable);
             builder.AddProduct(diamondProductId4, ProductType.Consumable);
             builder.AddProduct(diamondProductId5, ProductType.Consumable);
             builder.AddProduct(diamondProductId6, ProductType.Consumable);*/

            UnityPurchasing.Initialize(this, builder);
        }

        //public void BuyDiamond1()
        //{
        //    m_StoreController.InitiatePurchase(diamondProductId1);
        //}
        public void BuyRemoveAds()
        {
            m_StoreController.InitiatePurchase(removeAdsProductId);
        }
        /* public void BuyDiamond2()
         {
             m_StoreController.InitiatePurchase(diamondProductId2);
         }
         public void BuyDiamond3()
         {
             m_StoreController.InitiatePurchase(diamondProductId3);
         }
         public void BuyDiamond4()
         {
             m_StoreController.InitiatePurchase(diamondProductId4);
         }
         public void BuyDiamond5()
         {
             m_StoreController.InitiatePurchase(diamondProductId5);
         }
         public void BuyDiamond6()
         {
             m_StoreController.InitiatePurchase(diamondProductId6);
         }*/

        void AddDiamond1()
        {
            Debug.Log(1);
            //DataManager.instance.SetDiamond(100);
        }
        void RemoveAds()
        {
            Debug.Log("Ads removed");
            DataManager.instance.saveData.removeAds = true;
            DataManager.instance.RemoveAdsFunc();
            //DataManager.instance.SetDiamond(100);
        }
        /* void AddDiamond2()
         {
             Debug.Log(2);
             //DataManager.instance.SetDiamond(220);
         }
         void AddDiamond3()
         {
             Debug.Log(3);
             //DataManager.instance.SetDiamond(600);
         }
         void AddDiamond4()
         {
             Debug.Log(4);
             //DataManager.instance.SetDiamond(1300);
         }
         void AddDiamond5()
         {
             Debug.Log(5);
             //DataManager.instance.SetDiamond(2800);
         }
         void AddDiamond6()
         {
             Debug.Log(6);
             //DataManager.instance.SetDiamond(7500);
         }*/

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("In-App Purchasing successfully initialized");
            m_StoreController = controller;
        }
#nullable enable
        public void OnInitializeFailed(InitializationFailureReason error, string? message = null)
        {
            var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

            if (message != null)
            {
                errorMessage += $" More details: {message}";
            }

            Debug.Log(errorMessage);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            //Retrieve the purchased product
            var product = args.purchasedProduct;

            //Add the purchased product to the players inventory
            //if (product.definition.id == diamondProductId1)
            //{
            //    AddDiamond1();
            //}
            if(product.definition.id == removeAdsProductId)
            {
                RemoveAds();
            }
            /*
            else if(product.definition.id == diamondProductId3)
            {
                AddDiamond3();
            }
            else if(product.definition.id == diamondProductId4)
            {
                AddDiamond4();
            }
            else if(product.definition.id == diamondProductId5)
            {
                AddDiamond5();
            }
            else if(product.definition.id == diamondProductId6)
            {
                AddDiamond6();
            }*/

            Debug.Log($"Purchase Complete - Product: {product.definition.id}");

            //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            throw new NotImplementedException();
        }
    }
}
