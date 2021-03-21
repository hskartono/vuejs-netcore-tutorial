<template>
    <div>
        <b-form-group id="fieldset-is-received" v-bind="$attrs">
            <template v-slot:label>{{ labelStr }} <span v-if="isRequired" class="required"></span></template>
            <div v-if="isAllowEdit">
                <b-input-group>
                    <b-form-file
                        v-bind="$attrs"
                        v-model="inputModel"
                        @change="setUploadFile"
                        placeholder="Choose a file or drop it here..."
                        drop-placeholder="Drop file here..."
                        accept="image/*"
                        size="sm"
                    ></b-form-file>
                    <b-input-group-append v-if="imageFile != null">
                        <b-button variant="light" class="btn-open-camera" v-b-modal.show-image><b-icon-image-fill /></b-button>
                    </b-input-group-append>
                    <b-input-group-append v-if="imageFile == null && currentModel != null">
                        <b-button variant="light" class="btn-open-camera" v-b-modal.show-image><b-icon-image-fill /></b-button>
                    </b-input-group-append>
                </b-input-group>
                <b-form-invalid-feedback id="input-name-live-feedback">
                    {{ errorMessage }}
                </b-form-invalid-feedback>
            </div>
            <div v-else>
                {{ inputModel }}
            </div>
        </b-form-group>
        <b-col cols="12">
            <b-modal id="show-image" title="Image Uploaded" :hide-footer="true">
                <b-img v-if="imageFile" thumbnail fluid :src="imageFile" alt="Image 1"></b-img>
                <b-img v-if="currentModel && imageFile == null" thumbnail fluid :src="imageUrl" alt="Image 1"></b-img>
            </b-modal>
        </b-col>
    </div>
</template>
<script>
import { StreamBarcodeReader } from "vue-barcode-reader";
export default {
    components: {StreamBarcodeReader},
    props: ["model", "errorMessage", , "currentModel", "state", "isEditable", "maxFileSize", "label", "isRequired"],
    data() {
        return {
            labelStr: this.label,
            showImage: false,
            inputModel: this.model,
            imageFile: null
        }
    },
    methods: {
        setUploadFile(event) {
            if (event.target.files.length > 0) {
                let file = event.target.files[0];
                if (this.maxFileSize != null && this.maxFileSize != undefined) {
                    let fileSize = (file.size / 1048576).toFixed();
                    if (fileSize > this.maxFileSize) {
                        alert("Max Allowed File Size " + this.maxFileSize + " MB");
                        this.inputModel = null;
                        return;
                    }
                }
                this.inputModel = file;
                let reader = new FileReader();
                let this_ = this;
                reader.onload = function(e) {
                    this_.imageFile = e.target.result;
                }
                
                reader.readAsDataURL(this.inputModel);

                this.$emit('update:model', this.inputModel);
                this.$emit('change');
            }
        }
    },
    computed : {
        isAllowEdit : {
            get: function() {
                if (this.isEditable != undefined) return this.isEditable;
                else return true;
            }
        },
        imageUrl() {
            if (this.currentModel != null) {
                return this.currentModel.downloadUrl;
            }
        }
    },
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