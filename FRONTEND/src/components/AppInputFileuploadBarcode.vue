<template>
    <div>
        <b-form-group id="fieldset-is-received" v-bind="$attrs">
            <template v-slot:label>{{ labelStr }} <span v-if="isRequired" class="required"></span></template>
            <b-input-group>
                <b-form-input v-bind="$attrs" v-model="inputModel" @keyup="$emit('update:model', inputModel);" @keyup.enter="enterAction"></b-form-input>
                <b-input-group-append>
                    <b-button variant="light" class="btn-open-camera" v-b-modal.scan-barcode><b-icon-upc-scan /></b-button>
                </b-input-group-append>
            </b-input-group>
            <b-form-invalid-feedback id="input-name-live-feedback" :state="state">
                {{ errorMessage }}
            </b-form-invalid-feedback>
        </b-form-group>
        <b-col cols="12">
            <b-modal hide-footer ref="scan-barcode" id="scan-barcode" @ok="saveBarcodeText" @hidden="resetScanner" title="Barcode Scanner">
                <div v-if="!isScanned">
                    <StreamBarcodeReader
                    @decode="(a, b, c) => onDecode(a, b, c)"
                    @loaded="() => onLoaded()"
                    ></StreamBarcodeReader>
                    {{ scannedBarcode }}
                </div>
                <div v-if="isScanned">
                    <div class="barcode-text">{{ scannedBarcode }}</div>
                    <center>
                        <b-button variant="warning" @click="resetScanner()">Scan Ulang</b-button>
                    </center>
                </div>
            </b-modal>
        </b-col>
    </div>
</template>
<script>
import { StreamBarcodeReader } from "vue-barcode-reader";
export default {
    components: {StreamBarcodeReader},
    props: ["model", "state", "errorMessage", "isEditable", "scannedMethod", "label", "isRequired"],
    data() {
        return {
            labelStr: this.label,
            scannedBarcode: "",
            isScanned: false,
            inputModel: this.model,
        }
    }, 
    methods: {
        onDecode(a, b, c) {
            this.scannedBarcode = a;
            //this.isScanned = true;
            this.saveBarcodeText();
        },
        saveBarcodeText() {
            this.inputModel = this.scannedBarcode;
            this.$emit('update:model', this.inputModel);
            this.$bvModal.hide("scan-barcode");
            this.$emit('change');
            this.enterAction();
            this.resetScanner();
        },
        enterAction() {
            this.$emit("scannedMethod");
        },
        resetScanner() {
            this.scannedBarcode = "";
            this.isScanned = false;
        },
        onLoaded() {
            console.log("load");
        },
    },
    watch : {
        model : function(val) {
            this.inputModel = val;
        }
    }
}
</script>
<style lang="scss" scoped>
/deep/ .modal-dialog {
    height: 70vh;
    width: 60vh;
}
.input-group-text {
    font-size: 0.7rem !important;
}

.btn-open-camera {
    padding: 1px 6px !important;
}

.barcode-text {
    font-size: 2rem;
    text-align: center;
}

</style>