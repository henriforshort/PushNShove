using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;


public class IAP : MonoBehaviour, IStoreListener {
    public static IAP m;

    public static IStoreController storeController;
    public static IExtensionProvider storeExtensionProvider;

    [Serializable]
    public class Product {
        public ProductName name;
        public ProductType type;
        public Action effect;
        
        public Product(ProductName name, ProductType type, Action effect) {
            this.name = name;
            this.type = type;
            this.effect = effect;
        }
    }


    // ====================
    // ADD YOUR IAPs HERE, IGNORE THE REST
    // ====================

    // STEP 1: create the name of your products (= IAPs) in the enum
    // (some examples are provided, you can adapt or delete them)
    public enum ProductName { GEMS_50, GEMS_200, REMOVE_ADS, SEASON_PASS }
    

    
    // STEP 2: implement what each product does (after the player bought it succesfully) in a static method
    public static void gems50() { /* your code to add 50 gems here */ }
    public static void gems200() { /* your code to add 200 gems here */ }
    public static void removeAds() { /* your code to remove ads here */ }
    public static void seasonPass() { /* your code to get the season pass here */ }
    
    

    // STEP 3: add your product to the list, and specify its name, its type (consumable/non-consumable/subscription),
    // and its effect (the method to call when the player buys it)
    public List<Product> products = new List<Product> {
        new Product(ProductName.GEMS_50, ProductType.Consumable, gems50),
        new Product(ProductName.GEMS_200, ProductType.Consumable, gems200),
        new Product(ProductName.REMOVE_ADS, ProductType.NonConsumable, removeAds),
        new Product(ProductName.SEASON_PASS, ProductType.Subscription, seasonPass),
    };


    
    // STEP 4: Call the "IAP.m.BuyIAP" method from anywhere in your code to trigger the purchase sequence of a product.
    // Call the "IAP.m.RestorePurchases" to restore all non-consumable purchases (iphone only)
    // You can also create shortcut methods like these if you want:
    public void Buy50Gems() => BuyIAP(ProductName.GEMS_50);
    public void Buy200Gems() => BuyIAP(ProductName.GEMS_200);
    public void BuyRemoveAds() => BuyIAP(ProductName.REMOVE_ADS);
    public void BuySeasonPass() => BuyIAP(ProductName.SEASON_PASS);
    

    
    // STEP 5: set up each of these IAP in the App Store/Play Store
    
    
    
    
    // ====================
    // INIT
    // ====================
    
    public void Awake() {
        if (m == null) m = this;
        if (m != this) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        InitPurchases();
    }
    
    public void InitPurchases() {
        if (storeController != null) return;

        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        products.ForEach(p => builder.AddProduct(p.name.ToString(), p.type));
        UnityPurchasing.Initialize(this, builder);
    }

    //called through the IStoreListener interface
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        storeController = controller;
        storeExtensionProvider = extensions;
    }
    
    //called through the IStoreListener interface
    public void OnInitializeFailed(InitializationFailureReason e) => Debug.LogError($"IAP init failed: {e}");


    // ====================
    // BUY IAP
    // ====================

    public void BuyIAP(ProductName productName) {
        UnityEngine.Purchasing.Product product = storeController.products.WithID(productName.ToString());
        if (product != null && product.availableToPurchase) storeController.InitiatePurchase(product);
    }

    //called through the IStoreListener interface
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
        products.FirstOrDefault(p => args.purchasedProduct.definition.id == p.name.ToString())?.effect();
        return PurchaseProcessingResult.Complete;
    }

    //called through the IStoreListener interface
    public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason e) =>
        Debug.Log($"Failed to purchase '{product.definition.storeSpecificId}': {e}");                  


    // ====================
    // RESTORE PURCHASES
    // ====================
    
    public void RestorePurchases() {
        if (Application.platform != RuntimePlatform.IPhonePlayer && 
            Application.platform != RuntimePlatform.OSXPlayer) return;

        storeExtensionProvider
            .GetExtension<IAppleExtensions>()
            .RestoreTransactions(result => Debug.Log($"Restoring purchases: {result}"));
    }
}